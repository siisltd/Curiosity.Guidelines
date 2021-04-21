using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.Samples.WebApp.API.HostedServices
{
    /// <summary>
    /// Базовый класс для сервисов, которые выполняют какую-то функцию по таймеру.
    /// </summary>
    public abstract class WatchdogBase : BackgroundService
    {
        protected readonly ILogger Logger;

        protected WatchdogBase(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Starting {GetType().Name}...");
            await base.StartAsync(cancellationToken);
            Logger.LogInformation($"Starting {GetType().Name} completed.");
        }
        
        /// <summary>
        /// Исполняет переданную функцию в цикле с таймером.
        /// Безопасно ловит исключение и перезапускается через 10 минут.
        /// </summary>
        protected async Task ExecuteInLoopAsync(TimeSpan timer, CancellationToken stoppingToken, Func<Task> funcAsync)
        {
            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await funcAsync.Invoke();
                    await Task.Delay(timer, stoppingToken);
                }
                catch (Exception e) when (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning(e, $"Stopping {GetType().Name}");
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Critical error in {GetType().Name}");
                    
                    // чтоб не дубасил по 500 исключений в секунду, но и не умирал на вечно
                    Logger.LogInformation($"{GetType().Name} is waiting for 10 minutes and then is going to try again.");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                    Logger.LogInformation($"{GetType().Name} try to execute again.");
                }
            }
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Stopping {GetType().Name}...");
            await base.StopAsync(cancellationToken);
            Logger.LogInformation($"Stopping {GetType().Name} completed.");
        }
    }
}