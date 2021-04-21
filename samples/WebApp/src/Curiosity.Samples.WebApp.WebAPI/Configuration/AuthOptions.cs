using System;
using System.Collections.Generic;
using System.Text;
using Curiosity.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Curiosity.Samples.WebApp.API.Configuration
{
    /// <summary>
    /// Настройки JWT авторизации
    /// </summary>
    public class AuthOptions : IValidatableOptions, ILoggableOptions
    {
        /// <summary>
        /// Издатель токена
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Потребитель токена
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Уникальный ключ для генерации SymmetricSecurityKey
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Сколько дней живет токен
        /// </summary>
        public int LifeDays { get; set; }

        /// <summary>
        /// Получает SymmetricSecurityKey по текущему ключу
        /// </summary>
        public SymmetricSecurityKey SymmetricSecurityKey =>
            _symmetricSecurityKey ??= new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key ?? throw new ArgumentNullException(nameof(Key))));

        private SymmetricSecurityKey? _symmetricSecurityKey;

        /// <summary>
        /// Блокировать авторизацию, если ввели неправильный пароль?
        /// </summary>
        public bool IsLockoutEnable { get; set; } = true;

        /// <summary>
        /// Блокировать авторизацию на это время, сек
        /// </summary>
        public int LockoutTimeSec { get; set; } = 300;

        /// <summary>
        /// Количество неверных попыток авторизации до блокировки
        /// </summary>
        public int LockoutFailureCount { get; set; } = 5;

        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(Issuer), nameof(Issuer), "Не может быть пустым");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(Audience), nameof(Audience), "Не может быть пустым");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(Key), nameof(Key), "Не может быть пустым");
            errors.AddErrorIf(LifeDays < 1, nameof(LifeDays), "Не может быть меньше 1");
            errors.AddErrorIf(LockoutTimeSec < 1, nameof(LockoutTimeSec), "Не может быть меньше 1");
            errors.AddErrorIf(LockoutFailureCount < 1, nameof(LockoutFailureCount), "Не может быть меньше 1");

            return errors;
        }
    }
}