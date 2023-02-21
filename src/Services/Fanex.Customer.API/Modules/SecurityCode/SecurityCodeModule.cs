namespace Fanex.Customer.API.Modules.SecurityCode;

public static class SecurityCodeModule
{
    public static IServiceCollection RegisterSecurityCodeModule(this IServiceCollection services)
    {
        services.AddSingleton<ISecurityCodeService, SecurityCodeService>();

        return services;
    }
}