using System;
using Curiosity.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.DAL
{
    public class DataContextFactory : ICuriosityDataContextFactory<DataContext>
    {
        private readonly DbOptions _dbOptions;
        private readonly ILoggerFactory _loggerFactory;

        public DataContextFactory(DbOptions dbOptions, ILoggerFactory loggerFactory)
        {
            _dbOptions = dbOptions ?? throw new ArgumentNullException(nameof(dbOptions));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        ICuriosityDataContext ICuriosityDataContextFactory.CreateContext(bool isLoggingEnabled)
        {
            return CreateContext(isLoggingEnabled);
        }
        
        public DataContext CreateContext(bool isLoggingEnabled = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(_dbOptions.ConnectionString) // используем PostgreSQL database
                .UseLazyLoadingProxies();               // запрашиваем связанные сущности при обращении (если Include не использовался)
            
            if (_dbOptions.IsGlobalLoggingEnabled || isLoggingEnabled)
            {
                optionsBuilder.UseLoggerFactory(_loggerFactory);
                
                if (_dbOptions.IsSensitiveDataLoggingEnabled)
                    optionsBuilder.EnableSensitiveDataLogging();
            }
            
            return new DataContext(optionsBuilder.Options);
        }
    }
}