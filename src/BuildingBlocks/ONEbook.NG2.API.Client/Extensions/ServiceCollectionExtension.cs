using ONEbook.NG2.API.Client.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using ONEbook.NG2.API.Client.Services;

namespace ONEbook.NG2.API.Client.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddONEbookNG2APIClient(this IServiceCollection services, IConfiguration configuration)
    {
        var ng2Configuration = new NG2Configuration();
        configuration.GetSection(NG2Configuration.NG2).Bind(ng2Configuration);
        services.AddSingleton(ng2Configuration);

        services.AddSingleton<ING2StatusService, NG2StatusService>();
        services.AddHttpClient<ING2StatusService, NG2StatusService>();

        return services;
    }
}