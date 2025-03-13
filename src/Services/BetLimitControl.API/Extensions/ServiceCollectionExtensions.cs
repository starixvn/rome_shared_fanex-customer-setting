using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.RabbitMQ;

namespace BetLimitControl.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFanexLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var logEngines = new List<ILogEngine>
            {
                new RabbitMQLogging(configuration["Fanex.Logging:RabbitMQConnectionStrings"])
            };

            LogManager
                    .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
                    .Use(logEngines);

            services.AddSingleton(Logger.Log);

            ExceptionHandler
                .UseConfig(new LoggingConfiguration()
                .GetHttpHeaderInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo));

            return services;
        }
    }
}