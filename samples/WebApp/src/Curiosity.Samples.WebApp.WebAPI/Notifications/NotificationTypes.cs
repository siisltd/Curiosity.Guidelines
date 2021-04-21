namespace Curiosity.Samples.WebApp.API.Notifications
{
    /// <summary>
    /// Типы уведомления
    /// </summary>
    public enum NotificationTypes
    {
        Unknown = 0,
        
        /// <summary>
        /// Новый пользователь зарегистрирован в системе
        /// </summary>
        Registration = 1,
        
        /// <summary>
        /// Письмо с токеном для сброса пароля
        /// </summary>
        ResetPassword = 2,
    }
}