namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public static class PositionTakingModule
{
    public static IServiceCollection RegisterPositionTemplateModule(this IServiceCollection services)
    {
        services.AddSingleton<IPositionTakingTemplateService, PositionTakingTemplateService>();

        return services;
    }
}