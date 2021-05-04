using Curiosity.Notifications;

namespace Curiosity.Samples.WebApp.API.Notifications.User.Metadata
{
    /// <summary>
    /// Данные для создания сообщения о регистрации нового пользователя в системе
    /// </summary>
    public class RegistrationMetadata : BaseUserMetadata, INotificationMetadata
    {
        public string Type { get; } = NotificationTypes.Registration.ToString();
        
        public RegistrationMetadata(string email, string token) : base(email, token)
        {
        }
    }
}