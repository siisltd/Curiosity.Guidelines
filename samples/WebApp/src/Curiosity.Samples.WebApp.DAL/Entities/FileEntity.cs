using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Curiosity.Samples.WebApp.DAL.Entities
{
    /// <summary>
    /// Файл
    /// </summary>
    [Table("files")]
    public class FileEntity : BaseEntity
    {
        /// <summary>
        /// Id пользователя, которому пренадлежит файл
        /// </summary>
        [Column("user_id")]
        public long UserId { get; set; }
        
        /// <summary>
        /// Пользователь которому пренадлежит файл
        /// </summary>
        public virtual UserEntity? User { get; set; }
        
        /// <summary>
        /// Пользовательское имя файла
        /// </summary>
        [Column("user_file_name")]
        [StringLength(200)]
        [Required]
        public string UserFileName { get; set; } = null!;

        /// <summary>
        /// Путь к файлу в хранилище
        /// </summary>
        [Column("storage_file_path")]
        [StringLength(200)]
        [Required]
        public string StorageFilePath { get; set; } = null!;

        /// <summary>
        /// Имя файла в хранилище
        /// </summary>
        [Column("storage_file_name")]
        [StringLength(200)]
        [Required]
        public string StorageFileName { get; set; } = null!;
    }
}