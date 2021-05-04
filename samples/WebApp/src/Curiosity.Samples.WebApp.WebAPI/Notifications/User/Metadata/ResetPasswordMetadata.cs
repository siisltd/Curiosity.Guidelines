using Curiosity.Notifications;

namespace Curiosity.Samples.WebApp.API.Notifications.User.Metadata
{
    /// <summary>
    /// Данные для создания сообщения а сбросе пароля
    /// </summary>
    public class ResetPasswordMetadata : BaseUserMetadata, INotificationMetadata
    {
        public string Type { get; } = NotificationTypes.ResetPassword.ToString();

        public ResetPasswordMetadata(string email, string token) : base(email, token)
        {
        }
    }
}