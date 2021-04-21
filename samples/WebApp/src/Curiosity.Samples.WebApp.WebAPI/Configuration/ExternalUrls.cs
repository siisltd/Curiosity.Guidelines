using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Samples.WebApp.API.Configuration
{
    /// <summary>
    /// Список внешних адресов
    /// </summary>
    public class ExternalUrls : IValidatableOptions, ILoggableOptions
    {
        /// <summary>
        /// URL основного клиентского приложения
        /// </summary>
        public string WebSite { get; set; } = null!;

        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(WebSite), nameof(WebSite), "Не может быть пустым");

            return errors;
        }
    }
}