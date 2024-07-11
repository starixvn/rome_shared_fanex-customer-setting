using Fanex.Customer.API.Modules.PositionTaking;

namespace Fanex.Customer.API.Modules.Customer;

public static class CustomerCodeModule
{
    public static IServiceCollection RegisterCustomerModule(this IServiceCollection services)
    {
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<INicknameService, NicknameService>();
        services.AddSingleton<IProtectionCodeService, ProtectionCodeService>();
        services.AddSingleton<IProfilePictureService, ProfilePictureService>();
        services.AddSingleton<ILanguageService, LanguageService>();
        services.AddSingleton<IPositionTakingService, PositionTakingService>();

        return services;
    }
}