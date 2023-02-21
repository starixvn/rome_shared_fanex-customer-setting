using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.RemotingRef;

namespace Fanex.Customer.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFanexData(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
    {
        var connectionStrings = configuration
            .GetSection("ConnectionStrings")
            .GetChildren()
            .ToDictionary(
                connection => connection.Key,
                connection => new ConnectionConfiguration(connection.Key, connection.Value),
                StringComparer.OrdinalIgnoreCase);

        var resourcePath = Path.Combine(env.ContentRootPath, "App_Data", "SPNames.xml");
        DbSettingProviderManager
            .StartNewSession()
            .WithSqlServer(resourcePath)
            .Use(connectionStrings)
            .Run();
        return app;
    }

    public static IApplicationBuilder UseRemotingRef(this IApplicationBuilder app, IConfiguration configuration)
    {
        RefDataTypeContainer
            .ConnectTo(new Uri(configuration["RemoteRefURL"]))
            .FromApplication("Alpha.CloneAccount.Api");

        return app;
    }
}