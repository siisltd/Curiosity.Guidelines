using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Curiosity.Samples.WebApp.DAL.Entities
{
    /// <summary>
    /// Базовый класс для всех сущностей EF (кроме Identity)
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Id of entity
        /// </summary>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        /// <summary>
        /// Дата создания сущности в БД
        /// </summary>
        /// <remarks>
        /// Генерируется автоматически БД
        /// </remarks>
        [Column("created")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; }
    }
}