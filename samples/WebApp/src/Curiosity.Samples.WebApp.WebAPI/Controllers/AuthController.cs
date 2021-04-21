using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Curiosity.Notifications;
using Curiosity.Samples.WebApp.API.BLL;
using Curiosity.Samples.WebApp.API.BLL.Auth;
using Curiosity.Samples.WebApp.API.Configuration;
using Curiosity.Samples.WebApp.API.Exceptions;
using Curiosity.Samples.WebApp.API.Models;
using Curiosity.Samples.WebApp.API.Models.Auth;
using Curiosity.Samples.WebApp.API.Notifications.User.Metadata;
using Curiosity.Samples.WebApp.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.API.Controllers
{
    /// <summary>
    /// Авторизация
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly AuthService _authService;
        private readonly AppConfiguration _configuration;

        public AuthController(
            ILogger<AuthController> logger,
            SignInManager<UserEntity> signInManager, 
            AuthService authService, 
            AppConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Авторизация по паролю
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Allow anonymous
        /// </remarks>
        [HttpPost("password")]
        public async Task<Response<string>> Password([FromBody] PasswordRequestModel request)
        {
            const string errorMessage = "Неверный email или логин";

            var user = await _signInManager.UserManager.Users
                           .Where(x => x.NormalizedEmail == request.Email.ToUpper())
                           .Where(x => !x.IsDeleted)
                           .SingleOrDefaultAsync()
                       ?? throw new InvalidRequestDataException(errorMessage);

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, _configuration.AuthOptions.IsLockoutEnable);
            if (!result.Succeeded)
            {
                var lockout = _configuration.AuthOptions.LockoutTimeSec < 60 
                    ? $"{_configuration.AuthOptions.LockoutTimeSec} сек" 
                    : $"{_configuration.AuthOptions.LockoutTimeSec/60} мин";
                
                throw new InvalidRequestDataException(result.IsLockedOut
                    ? $"Пользователь заблокирован попробуйте через {lockout}"
                    : errorMessage);
            }

            var token = _authService.GenerateToken(user);

            _logger.LogInformation($"Пользователь (Id: {user.Id}, Email: \"{user.Email}\") успешно авторизовался по паролю");
            return new Response<string>(token);
        }
        
        /// <summary>
        /// Запрос на сброс пароля по токену (отправляет URL c токеном на почту)
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Allow anonymous
        /// </remarks>
        [HttpGet("password/token")]
        [AllowAnonymous]
        public async Task<Response<string>> RequestUpdatePasswordByToken(
            [MinLength(1)] string email,
            [FromServices] INotificator notificator)
        {
            _logger.LogInformation("Запрос на сброс пароля по email");
            
            var user = await _signInManager.UserManager.FindByEmailAsync(email.Trim());
            if (user != null)
            {
                var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
                notificator.NotifyAndForgot(new ResetPasswordMetadata(
                    user.Email,
                    token));
            }
            else
            {
                _logger.LogWarning($"Пользователь по email: \"{email}\" не найден. Письмо отправлено не будет.");
            }
            
            return new Response<string>("Если в системе есть аккаунт с таким электронным адресом, мы вышлем вам инструкции по сбросу пароля");
        }
        
        /// <summary>
        /// Обновление пароля по токену из почты
        /// </summary>
        /// <remarks>
        /// <h3/>Authorization: Allow anonymous
        /// </remarks>
        [HttpPost("password/token")]
        [AllowAnonymous]
        public async Task<Response> UpdatePasswordByToken([FromBody] TokenPasswordModel model)
        {
            _logger.LogInformation($"Обновление пароля по токену для пользователя с email: \"{model.Email}\"");
            
            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Не нашли пользователя по email. Подозрительно.");
                throw new InvalidRequestDataException("Недействительный токен"); // не будем раскрывать карты
            }

            var result = await _signInManager.UserManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                throw new InvalidRequestDataException(result.Errors);
            }

            // пользователь получил токен по почте, значит почта подтверждена
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _signInManager.UserManager.UpdateAsync(user);
            }
            
            _logger.LogInformation($"Пользователь (Id = {user.Id}, Email = {user.Email}) успешно обновил пароль по токену");
            return new Response();
        }
    }
}