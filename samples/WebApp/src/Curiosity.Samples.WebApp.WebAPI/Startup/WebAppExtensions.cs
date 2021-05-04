using System;
using Curiosity.Archiver.SharpZip;
using Curiosity.EMail.Smtp;
using Curiosity.Notifications;
using Curiosity.Notifications.EMail;
using Curiosity.Samples.WebApp.API.BLL;
using Curiosity.Samples.WebApp.API.BLL.Auth;
using Curiosity.Samples.WebApp.API.Configuration;
using Curiosity.Samples.WebApp.API.HostedServices;
using Curiosity.Samples.WebApp.API.Notifications.User;
using Curiosity.Samples.WebApp.DAL;
using Curiosity.TempFiles;
using Curiosity.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Samples.WebApp.API.Startup
{
    public static class WebAppExtensions
    {
        /// <summary>
        /// Регистрация в контейнере своих сервисов.
        /// </summary>
        public static IServiceCollection AddWebAppServices(this IServiceCollection services, AppConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // singletons
            services.AddSingleton(configuration.DbOptions);
            services.AddSingleton<DataContextFactory>();
            services.AddSingleton<UrlBuilder>();
            services.AddSingleton<FileService>();

            // scoped
            services.AddScoped<AuthService>(); // scoped, потому что использует identity сервисы

            // other
            services.AddLocalDateTimeService();
            services.AddNotifications(configuration);
            services.AddSharpZipArchiver(configuration.TempFiles);
            services.AddTempDirCleaner(configuration.TempFiles);

            // hosted services
            services.AddHostedService<MigrationService>();

            return services;
        }
        
        /// <summary>
        /// Добавляет нотификаторы
        /// </summary>
        private static IServiceCollection AddNotifications(this IServiceCollection services, AppConfiguration configuration)
        {
            services.AddSmtpEMailSender(configuration.SmtpEMailOptions);
            services.AddCuriosityEMailChannel();

            // builders
            services.AddSingleton<INotificationBuilder, RegistrationEmailBuilder>();
            services.AddSingleton<INotificationBuilder, ResetPasswordEmailBuilder>();

            return services;
        }
    }
}