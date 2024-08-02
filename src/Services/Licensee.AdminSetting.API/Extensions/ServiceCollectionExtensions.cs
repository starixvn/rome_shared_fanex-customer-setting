using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.RabbitMQ;
using Fanex.Logging.Sentry;

namespace Licensee.AdminSetting.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
                .HideSensitiveInfo([
                   "(?:password=)(?<paramValue>[^&|^\\s]+)",
                  "(\"password\":)(?<paramValue>[^,|^\r]+)",
                   "(?:seccode=)(?<paramValue>[^&|^\\s]+)",
                   "(\"seccode\":)(?<paramValue>[^,|^\r]+)",
                   "(?:securitycode=)(?<paramValue>[^&|^\\s]+)",
                   "(\"securitycode\":)(?<paramValue>[^,|^\r]+)"
                ])
                .GetHttpHeaderInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            return services;
        }
    }
}