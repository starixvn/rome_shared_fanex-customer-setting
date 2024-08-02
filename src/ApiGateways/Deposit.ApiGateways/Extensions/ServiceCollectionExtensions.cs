using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.RabbitMQ;
using Fanex.Logging.Sentry;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;

namespace Deposit.ApiGateways.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOneBookUm(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();

            return services;
        }

        public static IServiceCollection AddFanexLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var logEngines = new List<ILogEngine>
            {
                new RabbitMQLogging(configuration["Fanex.Logging:RabbitMQConnectionStrings"])
            };

            var sentryUrl = configuration["Fanex.Logging:SentryUrl"];
            if (!string.IsNullOrWhiteSpace(sentryUrl))
            {
                logEngines.Add(new SentryLogging(new SentryEngineOptions
                {
                    Dsn = sentryUrl
                }));
            }

            LogManager
                    .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
                    .Use(logEngines);

            services.AddSingleton(Logger.Log);

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .GetHttpHeaderInfo();

            return services;
        }
    }
}