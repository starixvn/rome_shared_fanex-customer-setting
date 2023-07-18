using Fanex.Customer.API.Modules.CopyCustomer.DCS;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public static class CopyCustomerCodeModule
{
    public static IServiceCollection RegisterCopyCustomerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDCSService, DCSService>();
        services.AddSingleton<ICopyMemberSpParamsBuilder, CopyMemberSpParamsBuilder>();
        services.AddSingleton<IThirdPartyProductService, ThirdPartyProductService>();
        services.AddSingleton<ICopyCustomerService, CopyCustomerService>();

        return services;
    }
}