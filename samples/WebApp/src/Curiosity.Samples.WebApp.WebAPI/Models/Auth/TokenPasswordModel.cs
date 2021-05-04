using System.ComponentModel.DataAnnotations;

namespace Curiosity.Samples.WebApp.API.Models.Auth
{
    /// <summary>
    /// Модель для сброса пароля по токену из письма
    /// </summary>
    public class TokenPasswordModel
    {
        /// <summary>
        /// Новый пароль
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
        
        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirmation { get; set; } = null!;
        
        [Required]
        [MinLength(1)]
        public string Token { get; set; } = null!;
        
        [Required]
        public string Email { get; set; } = null!;
    }
}