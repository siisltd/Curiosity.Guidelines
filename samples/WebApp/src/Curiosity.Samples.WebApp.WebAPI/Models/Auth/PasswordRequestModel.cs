using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Curiosity.Tools.Web.ModelBinders;

namespace Curiosity.Samples.WebApp.API.Models.Auth
{
    /// <summary>
    /// Модель для авторизации по паролю
    /// </summary>
    public class PasswordRequestModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        [JsonConverter(typeof(TrimStringConverter))] // автоматические обрезает пробелы у строки. Работает только для моделей [FromBody] и System.Text.Json.Serialization.JsonConverter
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Password { get; set; } = null!;
    }
}