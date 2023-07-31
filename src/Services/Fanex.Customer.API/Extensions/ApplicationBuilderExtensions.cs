using Fanex.Customer.API.Helpers;
using Fanex.Data;
using Fanex.Data.SqlServer;

namespace Fanex.Customer.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFanexData(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env, IServiceCollection services)
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
            .WithSqlServer()
            .Use(connectionStrings)
            .Use(new ShardDbSettingProvider(new ShardInfoAccessor(
                services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()),
                    resourcePath: resourcePath))
            .Run();
            
        return app;
    }
}