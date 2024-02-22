namespace Fanex.Customer.API.Modules.Customer;

public static class CustomerCodeModule
{
    public static IServiceCollection RegisterCustomerModule(this IServiceCollection services)
    {
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<INicknameService, NicknameService>();

        return services;
    }
}