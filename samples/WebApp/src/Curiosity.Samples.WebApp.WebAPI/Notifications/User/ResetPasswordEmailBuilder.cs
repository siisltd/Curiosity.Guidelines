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
    /// Собирает текст сообщения о сбросе пароля
    /// </summary>
    public class ResetPasswordEmailBuilder : EmailNotificationBuilderBase<ResetPasswordMetadata>
    {
        public override string NotificationType { get; } = NotificationTypes.ResetPassword.ToString();
        private readonly UrlBuilder _urlBuilder;
        private readonly ILogger<ResetPasswordEmailBuilder> _logger;

        public ResetPasswordEmailBuilder(UrlBuilder urlBuilder, ILogger<ResetPasswordEmailBuilder> logger)
        {
            _urlBuilder = urlBuilder ?? throw new ArgumentNullException(nameof(urlBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task<IReadOnlyList<INotification>> BuildNotificationsAsync(ResetPasswordMetadata metadata)
        {
            var subject = "Запрос на сброс пароля";
            var url = _urlBuilder.ResetPassword(metadata.Email, metadata.Token);
            
            var bodyBuilder = new StringBuilder();
            bodyBuilder
                .AppendLine($"<p>Здравствуйте, {metadata.Email}!</p>")
                .AppendNewLine()
                .AppendLine($"<p>Недавно Вы запросили восстановление пароля к вашему аккаунту в системе <b>{EMailBodyBuilderHelpers.SenderName}</b>.</p>")
                .AppendLine("<p>Если Вы не запрашивали восстановление пароля или сделали это случайно, можете не обращать внимания на это письмо.</p>")
                .AppendLine($"<p>Для сброса пароля перейдите по <a href=\"{url}\">ссылке</a>.</p>")
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