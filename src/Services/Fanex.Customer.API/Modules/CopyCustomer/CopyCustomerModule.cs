namespace Fanex.Customer.API.Modules.CopyCustomer;

public static class CopyCustomerCodeModule
{
    public static IServiceCollection RegisterCopyCustomerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICopyCustomerService, CopyCustomerService>();

        return services;
    }
}