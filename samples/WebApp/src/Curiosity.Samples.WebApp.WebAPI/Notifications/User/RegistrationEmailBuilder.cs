using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Curiosity.Notifications;
using Curiosity.Notifications.EMail;
using Curiosity.Samples.WebApp.API.BLL;
using Curiosity.Samples.WebApp.API.Notifications.User.Metadata;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.API.Notifications.User
{
    /// <summary>
    /// Собирает текст сообщения о регистрации нового пользователя
    /// </summary>
    public class RegistrationEmailBuilder : EmailNotificationBuilderBase<RegistrationMetadata>
    {
        public override string NotificationType { get; } = NotificationTypes.Registration.ToString();
        private readonly UrlBuilder _urlBuilder;
        private readonly ILogger<RegistrationEmailBuilder> _logger;

        public RegistrationEmailBuilder(UrlBuilder urlBuilder, ILogger<RegistrationEmailBuilder> logger)
        {
            _urlBuilder = urlBuilder ?? throw new ArgumentNullException(nameof(urlBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task<IReadOnlyList<INotification>> BuildNotificationsAsync(RegistrationMetadata metadata)
        {
            var subject = $"Регистрация в {EMailBodyBuilderHelpers.SenderName}";
            var url = _urlBuilder.ResetPassword(metadata.Email, metadata.Token);

            var bodyBuilder = new StringBuilder();
            bodyBuilder
                .AppendLine($"<p>Здравствуйте, {metadata.Email}!</p>")
                .AppendNewLine()
                .AppendLine($"<p>Вы успешно зарегистрировались в системе <b>{EMailBodyBuilderHelpers.SenderName}</b>.</p>")
                .AppendLine($"<p>Ваш логин: {metadata.Email}</p>")
                .AppendLine($"<p>Для завершения регистрации Вам необходимо задать пароль по <a href=\"{url}\">ссылке</a>.</p>")
                .AppendNewLine()
                .AppendEMailSignature();
            
            // это можно не логировать, но для примера, если отправка не работает, хотя-бы увидим сообщение и токен в логе
            _logger.LogDebug($"Создано новое email сообщение: \"{bodyBuilder}\"");
            _logger.LogDebug($"Email: \"{metadata.Email}\", Token: \"{metadata.Token}\"");

            return Task.FromResult<IReadOnlyList<INotification>>(new[]
            {
                new EmailNotification(
                    metadata.Email,
                    subject,
                    bodyBuilder.ToString(),
                    true)
            });
        }
    }
}