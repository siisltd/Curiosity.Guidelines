using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Migrations;
using Curiosity.Migrations.PostgreSQL;
using Curiosity.Samples.WebApp.API.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.API.HostedServices
{
    /// <summary>
    /// Проверяет и запускает миграции при старте приложения
    /// </summary>
    public class MigrationService : IHostedService
    {
        private readonly ILogger<MigrationService> _logger;
        private readonly AppConfiguration _configuration;
        private readonly IServiceCollection _services;

        public MigrationService(ILogger<MigrationService> logger, AppConfiguration configuration, IServiceCollection services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task StartAsync(CancellationToken cToken)
        {
            _logger.LogInformation("Мигратор запущен");
            
            var builder = new MigratorBuilder(_services);
            builder.UsePostgreSQL(
                _configuration.MigrationOptions.ConnectionString,
                lcCollate: _configuration.MigrationOptions.LC_Collate,
                lcCtype: _configuration.MigrationOptions.LC_Type,
                databaseEncoding: _configuration.MigrationOptions.DataBaseEncoding,
                migrationTableHistoryName: "migration_history");
            builder.UseScriptMigrations()
                .FromAssembly(typeof(MigrationService).Assembly, "Migrations.Scripts.");
            builder.UseCodeMigrations()
                .FromAssembly<IMigration>(typeof(MigrationService).Assembly);
            builder.UseUpgradeMigrationPolicy(MigrationPolicy.Allowed);
            builder.UserLogger(_logger);
            
            if (_configuration.MigrationOptions.LogSql)
            {
                builder.UseLoggerForSql(_logger);
            }
            
            if (!String.IsNullOrWhiteSpace(_configuration.MigrationOptions.GrantUser))
            {
                builder.UseVariable(DefaultVariables.User, _configuration.MigrationOptions.GrantUser);
            }
            
            var migrator = builder.Build();
            var result = await migrator.MigrateSafeAsync(cToken);

            _logger.LogInformation(result.IsSuccessfully
                ? "Миграции успешно выполнены"
                : $"Ошибка миграции: {result.ErrorMessage}");
        }

        public Task StopAsync(CancellationToken cToken)
        {
            return Task.CompletedTask;
        }
    }
}