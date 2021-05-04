using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Curiosity.EMail;
using Curiosity.Tools.Web.ModelBinders;

namespace Curiosity.Samples.WebApp.API.Models.Users
{
    public class UserEditorModel
    {
        /// <summary>
        /// Id пользователя.
        /// Для создания нового пользователя, должен быть null.
        /// </summary>
        public long? Id { get; set; }
        
        /// <summary>
        /// Дата создания пользователя
        /// </summary>
        [ReadOnly(true)]
        public DateTime? Created { get; set; }

        /// <summary>
        /// Электронная почта.
        /// Обязательна, для нового пользователя
        /// </summary>
        [MaxLength(256)]
        [JsonConverter(typeof(TrimStringConverter))]
        [ValidEmail(ErrorMessage = "Не корректный e-mail")]
        public string? Email { get; set; } = null!;

        /// <summary>
        /// Электронная почта подтверждена?
        /// </summary>
        [ReadOnly(true)]
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Номер мобильного телефона.
        /// Подойдет любой формат, приводит к виду 70000000000
        /// </summary>
        [JsonConverter(typeof(TrimStringConverter))]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Часовой пояс
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string TimeZoneId { get; set; } = null!;

        /// <summary>
        /// Удалён?
        /// </summary>
        public bool IsDeleted { get; set; }
        
    }
}