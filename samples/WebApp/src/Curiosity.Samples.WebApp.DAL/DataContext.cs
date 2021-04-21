using Curiosity.DAL.EF;
using Curiosity.Samples.WebApp.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Curiosity.Samples.WebApp.DAL
{
    public class DataContext : CuriosityDataContext<DataContext>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        /// <summary>
        /// Пользователи
        /// </summary>
        public virtual DbSet<UserEntity> Users { get; set; } = null!;
        
        /// <summary>
        /// Файлы
        /// </summary>
        public virtual DbSet<FileEntity> Files { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FileEntity>(entity =>
            {
                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId);
            });
        }
    }
}