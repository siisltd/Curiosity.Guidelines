using Curiosity.Samples.WebApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Curiosity.Samples.WebApp.DAL
{
    /// <summary>
    /// Отдельный контекст для корректной работы Identity
    /// </summary>
    /// <remarks>
    /// Создан отдельно, потому что Identity требует наследования от класса,
    /// а <see cref="DataContext"/> уже наследуется от другого класса.
    /// </remarks>
    public class IdentityDataContext : IdentityDbContext<UserEntity, IdentityRole<long>, long>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
        {
        }
    }
}