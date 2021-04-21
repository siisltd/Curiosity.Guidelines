using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Samples.WebApp.API.Configuration
{
    public class MigrationOptions : IValidatableOptions, ILoggableOptions
    {
        /// <summary>
        /// Строка подключения для мигратора
        /// </summary>
        public string ConnectionString { get; set; } = null!;

        public string DataBaseEncoding { get; set; } = null!;

        public string LC_Collate { get; set; } = null!;

        public string LC_Type { get; set; } = null!;

        /// <summary>
        /// Пользователь postgres которому передаются права на таблицу после создания
        /// </summary>
        public string? GrantUser { get; set; }

        /// <summary>
        /// Нужно ли логировать SQL запросы
        /// </summary>
        public bool LogSql { get; set; }

        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(ConnectionString), nameof(ConnectionString), "Не может быть пустым");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(DataBaseEncoding), nameof(DataBaseEncoding), "Не может быть пустым");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(LC_Collate), nameof(LC_Collate), "Не может быть пустым");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(LC_Type), nameof(LC_Type), "Не может быть пустым");

            return errors;
        }
    }
}