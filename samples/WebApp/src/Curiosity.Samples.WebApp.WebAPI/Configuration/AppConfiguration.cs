using System.Collections.Generic;
using Curiosity.Configuration;
using Curiosity.DAL;
using Curiosity.EMail.Smtp;
using Curiosity.Hosting.Web;
using Curiosity.TempFiles;
using Curiosity.Tools;
using Curiosity.Tools.Web.ReverseProxy;

namespace Curiosity.Samples.WebApp.API.Configuration
{
    /// <summary>
    /// Конфигурация приложения
    /// </summary>
    public class AppConfiguration : CuriosityWebAppConfiguration
    {
        public AppConfiguration()
        {
            AppName ??= "Sample Web App";
            Culture = new CultureOptions {DefaultCulture = "ru"};
        }

        /// <summary>
        /// Список внешних url
        /// </summary>
        public ExternalUrls ExternalUrls { get; set; } = new ExternalUrls();

        public ReverseProxyOptions? ReverseProxy { get; set; }
        public DbOptions DbOptions { get; set; } = new DbOptions();
        public MigrationOptions MigrationOptions { get; set; } = new MigrationOptions();

        /// <summary>
        /// Пользователи по умолчанию, которые создаются первой миграцией
        /// </summary>
        public List<DefaultUser> DefaultUsers { get; set; } = new List<DefaultUser>();

        /// <summary>
        /// Признак расширенного логирования входящих запросов
        /// </summary>
        public bool LogRequestContent { get; set; }
        
        public TempFileOptions TempFiles { get; set; } = new TempFileOptions();

        /// <summary>
        /// Настройки JWT авторизации
        /// </summary>
        public AuthOptions AuthOptions { get; set; } = new AuthOptions();
        public ISmtpEMailOptions SmtpEMailOptions { get; set; } = new SmtpEMailOptions();

        public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrors(base.Validate(prefix));
            if (ReverseProxy != null)
            {
                errors.AddErrors(ReverseProxy.Validate(nameof(ReverseProxy)));
            }

            errors.AddErrors(ExternalUrls.Validate(nameof(ExternalUrls)));
            errors.AddErrors(DbOptions.Validate(nameof(DbOptions)));
            errors.AddErrors(MigrationOptions.Validate(nameof(MigrationOptions)));
            errors.AddErrors(AuthOptions.Validate(nameof(AuthOptions)));
            errors.AddErrors(SmtpEMailOptions.Validate(nameof(SmtpEMailOptions)));
            errors.AddErrors(TempFiles.Validate(nameof(TempFiles)));

            return errors;
        }
    }
}