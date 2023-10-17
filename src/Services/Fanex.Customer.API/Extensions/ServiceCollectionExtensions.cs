using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.RabbitMQ;
using Fanex.Logging.Sentry;
using Fanex.RemotingRef;
using Sentry;

namespace Fanex.Customer.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFanexRemotingRef(this IServiceCollection services, IConfiguration configuration)
    {
        Fanex.RemotingRef.RefDataTypeContainer
            .ConnectTo(new Uri(configuration["Fanex.RemotingRef.Service"]))
            .FromApplication("Fanex.Customer.API");

        services.AddSingleton(new RemotingRefService());
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
                Dsn = new Dsn(sentryUrl)
            }));
        }

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