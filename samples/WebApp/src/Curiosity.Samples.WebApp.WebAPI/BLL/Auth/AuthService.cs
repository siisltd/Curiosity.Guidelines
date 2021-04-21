using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Curiosity.Samples.WebApp.API.Configuration;
using Curiosity.Samples.WebApp.API.Exceptions;
using Curiosity.Samples.WebApp.DAL;
using Curiosity.Samples.WebApp.DAL.Entities;
using Curiosity.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Curiosity.Samples.WebApp.API.BLL.Auth
{
    public class AuthService
    {
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly DataContextFactory _contextFactory;
        private readonly AppConfiguration _configuration;
        private readonly IDateTimeService _dateTimeService;
        private static readonly JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
        
        public AuthService(
            SignInManager<UserEntity> signInManager,
            DataContextFactory contextFactory,
            AppConfiguration configuration,
            IDateTimeService dateTimeService)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        }

        public string GenerateToken(UserEntity user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            var now = _dateTimeService.GetCurrentTimeUtc();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //new Claim(ClaimTypes.Name, user.FIO),
                //new Claim(ClaimTypes.Role, user.Client.Role), 
                //new Claim(ClientIdClimeName, user.Client.Id.ToString()),
                new Claim(ClaimTypes.Version, "1"), // версия токена. При добавлении новых claim - версия + 1 (для фронта)
            };

            var jwt = new JwtSecurityToken(
                issuer: _configuration.AuthOptions.Issuer,
                audience: _configuration.AuthOptions.Audience,
                notBefore: now,
                expires: now.AddDays(_configuration.AuthOptions.LifeDays),
                claims: claims,
                signingCredentials: new SigningCredentials(_configuration.AuthOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            return TokenHandler.WriteToken(jwt);
        }

        public long GetUserId(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            
            var claimValue = _signInManager.UserManager.GetUserId(httpContext.User) ?? "";
            if (!long.TryParse(claimValue, out var userId))
            {
                throw new InvalidOperationException($"Невозможно получить id пользователя из claims (claim value = \"{claimValue}\")");
            }

            return userId;
        }

        /// <summary>
        /// Возвращает информацию о текущем пользователе, которую можно использовать внутри бизнес логики.
        /// Используется кеширование.
        /// </summary>
        public async Task<UserInfo> GetCachedUserAsync(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            // TODO: Add curiosity caching
            
            var userId = GetUserId(httpContext);
            using (var context = _contextFactory.CreateContext())
            {
                var user = await context.Users
                               .Where(x => x.Id == userId)
                               .Where(x => !x.IsDeleted) // сразу проверяет заблокирован пользователь или нет
                               //.Where(x => !x.Client!.IsBlocked)
                               .Select(x => new UserInfo
                               {
                                   Id = x.Id,
                                   Email = x.Email,
                                   TimeZoneId = x.TimeZoneId,
                               })
                               .SingleOrDefaultAsync()
                           ?? throw new AuthException("Пользователь удалён или заблокирован");

                return user;
            }
        }
    }
}