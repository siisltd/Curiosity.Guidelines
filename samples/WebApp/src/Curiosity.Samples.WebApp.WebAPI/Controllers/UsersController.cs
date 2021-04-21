using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Curiosity.Notifications;
using Curiosity.Samples.WebApp.API.BLL.Auth;
using Curiosity.Samples.WebApp.API.Exceptions;
using Curiosity.Samples.WebApp.API.Models;
using Curiosity.Samples.WebApp.API.Models.Users;
using Curiosity.Samples.WebApp.API.Notifications.User.Metadata;
using Curiosity.Samples.WebApp.DAL;
using Curiosity.Samples.WebApp.DAL.Entities;
using Curiosity.TimeZone;
using Curiosity.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.API.Controllers
{
    /// <summary>
    /// Просмотр и редактирование пользователей
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly DataContextFactory _contextFactory;
        private readonly AuthService _authService;

        public UsersController(
            ILogger<UsersController> logger,
            DataContextFactory contextFactory,
            AuthService authService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Возвращает список всех пользователей
        /// </summary>
        /// <remarks>
        /// <h3/> Authorization: Required
        /// </remarks>
        [HttpGet]
        [Authorize]
        public async Task<Response<UserListModel[]>> GetUsers()
        {
            var currentUser = await _authService.GetCachedUserAsync(HttpContext);

            using (var context = _contextFactory.CreateContext())
            {
                var users = await context.Users
                    .Select(x => new UserListModel
                    {
                        Id = x.Id,
                        Email = x.Email,
                        IsDeleted = x.IsDeleted,
                    })
                    .OrderBy(x => x.Id)
                    .ToArrayAsync();

                return new Response<UserListModel[]>(users);
            }
        }

        /// <summary>
        /// Возвращает детальную информацию пользователя
        /// </summary>
        /// <remarks>
        /// <h3/> Authorization: Required
        /// </remarks>
        /// <param name="userId">Id пользователя</param>
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<Response<UserEditorModel>> GetUserEditor(long userId)
        {
            var currentUser = await _authService.GetCachedUserAsync(HttpContext);
            using (var context = _contextFactory.CreateContext())
            {
                var user = await context.Users
                               .Where(x => x.Id == userId)
                               .Select(src => new UserEditorModel
                               {
                                   Id = src.Id,
                                   Created = src.Created,
                                   Email = src.Email,
                                   EmailConfirmed = src.EmailConfirmed,
                                   PhoneNumber = src.PhoneNumber,
                                   TimeZoneId = src.TimeZoneId,
                                   IsDeleted = src.IsDeleted,
                               })
                               .SingleOrDefaultAsync()
                           ?? throw new EntityNotFoundException($"Пользователь (id = {userId}) не найден");

                return new Response<UserEditorModel>(user);
            }
        }

        /// <summary>
        /// Создаёт нового или редактирует существующего пользователя
        /// </summary>
        /// <remarks>
        /// <h3/> Authorization: Required
        /// </remarks>
        [HttpPost]
        [Authorize]
        public async Task<Response> CreateOrUpdate(
            [FromBody] UserEditorModel model,
            [FromServices] UserManager<UserEntity> userManager,
            [FromServices] INotificator notificator)
        {
            UserEntity user;
            if (model.Id.HasValue)
            {
                user = await userManager.Users
                           .Where(x => x.Id == model.Id.Value)
                           .SingleOrDefaultAsync()
                       ?? throw new EntityNotFoundException($"Пользователь (id = {model.Id.Value}) не найден");
            }
            else
            {
                user = new UserEntity();
            }
            
            // часовой пояс
            if (!TimeZoneHelper.IsTimeZoneIdValid(model.TimeZoneId))
                throw new InvalidRequestDataException("Не корректный часовой пояс");

            user.TimeZoneId = model.TimeZoneId;

            // телефон
            if (!String.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                if (!model.PhoneNumber.CleanupPhone().IsMobilePhoneFormat())
                    throw new InvalidRequestDataException("Не корректный номер телефона", nameof(model.PhoneNumber));

                user.PhoneNumber = model.PhoneNumber.ResolvePhoneTo7Format();
            }

            // email
            if (!model.Id.HasValue)
            {
                if (String.IsNullOrWhiteSpace(model.Email))
                    throw new InvalidRequestDataException("Email обязателен для нового пользователя", nameof(model.Email));

                user.Email = model.Email;
                user.UserName = model.Email;
            }

            // восстановление пользователя
            if (!user.IsDeleted && user.IsDeleted)
            {
                user.IsDeleted = model.IsDeleted;
                _logger.LogInformation($"Восстанавливаем пользователя (Id = {user.Id}) из удалённых");
            }

            if (model.Id.HasValue)
            {
                // обновляем
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new InvalidRequestDataException(result.Errors);

                _logger.LogInformation($"Успешно обновлен пользователь (Id = {model.Id})");
            }
            else
            {
                // создаём
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new InvalidRequestDataException(result.Errors);

                // для завершении регистрации пользователь должен задать пароль
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                notificator.NotifyAndForgot(new RegistrationMetadata(
                    user.Email,
                    token));

                _logger.LogInformation($"Успешно создан новый пользователь (Id = {user.Id}, email = {user.Email})");
            }

            return new Response();
        }

        /// <summary>
        /// Удаляет выбранных пользователей
        /// </summary>
        /// <remarks>
        /// <h3/> Authorization: Required
        /// </remarks>
        /// <param name="ids">Список id пользователей</param>
        [HttpDelete]
        [Authorize]
        public async Task<Response> DeleteUser([FromBody] [MinLength(1)] long[] ids)
        {
            var currentUser = await _authService.GetCachedUserAsync(HttpContext);
            _logger.LogInformation($"{currentUser} начал удаление {ids.Length} пользователей");

            using (var context = _contextFactory.CreateContext())
            {
                // удаление
                var deletedCount = 0;
                foreach (var id in ids)
                {
                    var user = await context.Users
                        .SingleOrDefaultAsync(u => u.Id == id);

                    if (user == null)
                        throw new EntityNotFoundException($"Не найден пользователь (id = {id})");

                    if (user.Id == currentUser.Id)
                        throw new InvalidRequestDataException("Нельзя удалять текущего пользователя");

                    if (user.IsDeleted)
                    {
                        _logger.LogWarning($"Пользователь (Id = {id}) уже удалён");
                        continue;
                    }

                    user.IsDeleted = true;
                    deletedCount++;
                }

                await context.SaveChangesAsync();
                _logger.LogInformation($"Успешно удалено {deletedCount} пользователей");
                return new Response();
            }
        }
    }
}