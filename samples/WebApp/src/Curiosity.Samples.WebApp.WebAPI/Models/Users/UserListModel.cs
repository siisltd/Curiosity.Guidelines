namespace Curiosity.Samples.WebApp.API.Models.Users
{
    /// <summary>
    /// Модель для отображения пользователей в списке
    /// </summary>
    public class UserListModel
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}