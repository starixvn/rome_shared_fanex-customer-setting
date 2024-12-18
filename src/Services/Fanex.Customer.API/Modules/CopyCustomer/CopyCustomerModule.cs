namespace Fanex.Customer.API.Modules.CopyCustomer;

public static class CopyCustomerCodeModule
{
    public static IServiceCollection RegisterCopyCustomerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ICopyCustomerService, CopyCustomerService>();

        return services;
    }
}