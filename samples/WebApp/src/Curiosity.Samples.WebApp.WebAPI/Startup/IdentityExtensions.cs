﻿using System;
using Curiosity.Samples.WebApp.API.Configuration;
using Curiosity.Samples.WebApp.DAL;
using Curiosity.Samples.WebApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Samples.WebApp.API.Startup
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// Добавляет сервисы Identity с нашими настройками
        /// </summary>
        public static IServiceCollection AddAppIdentity(this IServiceCollection services, AppConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            services.AddDbContext<IdentityDataContext>(opt => opt.UseNpgsql(configuration.DbOptions.ConnectionString));
            services
                .AddIdentityCore<UserEntity>(opt =>
                {
                    //opt.SignIn.RequireConfirmedAccount = true;
                    opt.SignIn.RequireConfirmedEmail = true;
                })
                .AddRoles<IdentityRole<long>>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<RuIdentityErrorDescriber>();

            services.AddScoped<SignInManager<UserEntity>>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                if (configuration.AuthOptions.IsLockoutEnable)
                {
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(configuration.AuthOptions.LockoutTimeSec);
                    options.Lockout.MaxFailedAccessAttempts = configuration.AuthOptions.LockoutFailureCount;
                    options.Lockout.AllowedForNewUsers = true;
                }

                // User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }
    }
    
    /// <summary>
    /// Русские сообщения об ошибках
    /// </summary>
    public class RuIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = "Произошла неизвестная ошибка" }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Ошибка оптимистичного контроля параллелизма, объект был изменён" }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "Некорретный пароль" }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "Недействительный токен" }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Пользователь с таким логином уже существует" }; }
        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"Имя пользователя \"{userName}\" некорректно, может содержать только буквы и цифры" }; }
        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"Email \"{email}\" некорректен" }; }
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Пользователь с именем \"{userName}\" уже существует" }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Email \"{email}\" уже используется" }; }
        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"Имя роли \"{role}\" некорректно" }; }
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Имя роли \"{role}\" уже используется" }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Пользователь уже установил пароль" }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Блокировка недоступна для этого пользователя" }; }
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"Пользователю уже присвоена роль \"{role}\"." }; }
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"У пользователя нет роли \"{role}\"." }; }
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Пароль должен быть длиной не менее {length} символов" }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Пароль должен содержать хотя бы один не буквенно-цифровой символ" }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать хотя бы одну цифру ('0'-'9')." }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Пароль должен содержать хотя бы один символ в нижнем регистре ('a'-'z')." }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать хотя бы один символ в верхнем регистре ('A'-'Z')" }; }
    }
}