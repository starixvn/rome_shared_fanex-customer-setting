using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.EditCustomer.Services;
using Fanex.Customer.API.Modules.QueryCustomer.Services;

namespace Fanex.Customer.API.Modules.CreateCustomer;

public static class CreateCustomerModule
{
    public static IServiceCollection RegisterCreateCustomerModule(this IServiceCollection services)
    {
        services.AddSingleton<IQueryCustomerService, QueryCustomerService>();
        services.AddSingleton<ICreateCustomerService, CreateCustomerService>();
        services.AddSingleton<IQueryGroupTypeBaseService<WinLossLimitItem>, QueryGroupTypeBaseService<WinLossLimitItem>>();
        services.AddSingleton<IEditCustomerService, EditCustomerService>();
        services.AddSingleton<IPTSettingService, PTSettingService>();

        //services.AddSingleton<ICreateSuperFullModeService, CreateSuperFullModeService>();

        return services;
    }
}