using Fanex.Customer.API.Modules.CreateCustomer.Services;

namespace Fanex.Customer.API.Modules.CreateCustomer;

public static class CreateCustomerModule
{
    public static IServiceCollection RegisterCreateCustomerModule(this IServiceCollection services)
    {
        services.AddSingleton<IQueryCustomerService, QueryCustomerService>();
        services.AddSingleton<ICreateCustomerService, CreateCustomerService>();

        return services;
    }
}