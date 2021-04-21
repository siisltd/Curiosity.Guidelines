using System;
using System.ComponentModel.DataAnnotations.Schema;
using Curiosity.Samples.WebApp.Common;
using Microsoft.AspNetCore.Identity;

namespace Curiosity.Samples.WebApp.DAL.Entities
{
    [Table("AspNetUsers")]
    public class UserEntity : IdentityUser<long>
    {
        /// <summary>
        /// Дата создания пользователя в БД
        /// </summary>
        /// <remarks>
        /// Генерируется автоматически БД
        /// </remarks>
        [Column("Created")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; }
        
        /// <summary>
        /// Пользователь удалён?
        /// </summary>
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// Пол
        /// </summary>
        [Column("Sex")]
        public SexType Sex { get; set; }
        
        
        /// <summary>
        /// Часовой пояс
        /// </summary>
        [Column("TimeZoneId")]
        public string TimeZoneId { get; set; } = null!;
    }
}