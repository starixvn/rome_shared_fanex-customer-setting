using Fanex.Customer.API.Modules.LiteModeCustomer.Services;

namespace Fanex.Customer.API.Modules.LiteModeCustomer;

public static class LiteModeModule
{
    public static IServiceCollection RegisterLiteModeModule(this IServiceCollection services)
    {
        services.AddSingleton<ILiteModeCustomerService, LiteModeCustomerService>();

        return services;
    }
}