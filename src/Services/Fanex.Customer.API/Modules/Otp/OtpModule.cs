namespace Fanex.Customer.API.Modules.Otp;

public static class OtpModule
{
    public static IServiceCollection RegisterOtpModule(this IServiceCollection services)
    {
        services.AddScoped<IOtpService, OtpService>();

        return services;
    }
}