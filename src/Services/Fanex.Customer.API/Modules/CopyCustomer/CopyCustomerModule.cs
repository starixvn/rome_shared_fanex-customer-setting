using Fanex.Customer.API.Modules.CopyCustomer.DCS;
using Fanex.Customer.API.Modules.CopyCustomer.NG2;
using ONEbook.NG2.API.Client.Extensions;
using ONEbook.UM.ThirdParty.AspNetCore.Extensions;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public static class CopyCustomerCodeModule
{
    public static IServiceCollection RegisterCopyCustomerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddONEbookUMThirdParty(configuration);
        services.AddONEbookNG2APIClient(configuration);

        services.AddSingleton<ING2Service, NG2Service>();
        services.AddSingleton<IDCSService, DCSService>();
        services.AddSingleton<ICopyMemberSpParamsBuilder, CopyMemberSpParamsBuilder>();
        services.AddSingleton<IThirdPartyProductService, ThirdPartyProductService>();
        services.AddSingleton<ICopyCustomerService, CopyCustomerService>();

        return services;
    }
}