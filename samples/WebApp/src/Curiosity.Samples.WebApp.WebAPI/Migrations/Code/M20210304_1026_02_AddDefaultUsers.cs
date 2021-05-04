using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Migrations;
using Curiosity.Samples.WebApp.API.Configuration;
using Curiosity.Samples.WebApp.Common;
using Curiosity.Samples.WebApp.DAL;
using Curiosity.Samples.WebApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.API.Migrations.Code
{
    /// <summary>
    /// Кодовая миграция, которая создаёт первых пользователей (например админов)
    /// </summary>
    public class M20210304_1026_02_AddDefaultUsers : CodeMigration
    {
        private readonly ILogger _logger;
        private readonly AppConfiguration _configuration;
        private readonly UserManager<UserEntity> _userManager;
        private readonly DataContextFactory _contextFactory;
        
        public override DbVersion Version => new DbVersion("20210304-1026.02");
        public override string Comment => "added_default_identity_users";

        public M20210304_1026_02_AddDefaultUsers(
            ILogger logger, 
            AppConfiguration configuration, 
            UserManager<UserEntity> userManager, 
            DataContextFactory contextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public override async Task UpgradeAsync(DbTransaction? transaction = null, CancellationToken token = new CancellationToken())
        {
            await using var context = _contextFactory.CreateContext();
            foreach (var defaultUser in _configuration.DefaultUsers)
            {
                // создадим пользователей
                var user = new UserEntity
                {
                    Email = defaultUser.Email,
                    EmailConfirmed = true,
                    UserName = defaultUser.Email,
                    Sex = SexType.Male,
                    TimeZoneId = Constants.DefaultTimeZoneId,
                };

                if (await _userManager.Users.AnyAsync(x => x.NormalizedEmail == user.Email.ToUpper(), cancellationToken: token))
                {
                    _logger.LogWarning($"Пользователь \"{user.Email}\" уже существует");
                    continue;
                }

                var result = await _userManager.CreateAsync(user, defaultUser.Password);
                if (!result.Succeeded)
                {
                    foreach (var resultError in result.Errors)
                    {
                        Logger.LogError($"{resultError.Code}: {resultError.Description}");
                    }

                    throw new MigrationException(MigrationError.MigratingError, $"Failed to add user \"{defaultUser.Email}\"");
                }
            }
        }
    }
}