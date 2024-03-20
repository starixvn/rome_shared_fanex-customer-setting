using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.EditCustomer.Services;

namespace Fanex.Customer.API.Modules.CreateCustomer;

public static class CreateCustomerModule
{
    public static IServiceCollection RegisterCreateCustomerModule(this IServiceCollection services)
    {
        services.AddSingleton<IQueryCustomerService, QueryCustomerService>();
        services.AddSingleton<ICreateCustomerService, CreateCustomerService>();
        services.AddSingleton<IEditCustomerService, EditCustomerService>();
        services.AddSingleton<IPTSettingService, PTSettingService>();
        services.AddSingleton<ISiteService, SiteService>();
        services.AddSingleton<ICurrencyService, CurrencyService>();
        services.AddSingleton<INoteMessageService, NoteMessageService>();

        return services;
    }
}