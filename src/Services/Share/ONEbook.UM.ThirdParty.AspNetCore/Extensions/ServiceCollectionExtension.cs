using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.ThirdParty.AspNetCore.Configurations;
using ONEbook.UM.ThirdParty.AspNetCore.Services;

namespace ONEbook.UM.ThirdParty.AspNetCore.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddONEbookUMThirdParty(this IServiceCollection services, IConfiguration configuration)
    {
        var umClientConfiguration = new UMClientConfiguration();
        configuration.GetSection(UMClientConfiguration.ThirdPartyUM).Bind(umClientConfiguration);
        services.AddSingleton(umClientConfiguration);

        services.AddSingleton<IUMService, UMService>();
        services.AddHttpClient<IUMService, UMService>();

        return services;
    }
}