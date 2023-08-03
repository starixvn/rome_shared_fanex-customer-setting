namespace Fanex.Customer.API.Modules.Rules
{
    public static class RulesServiceModule
    {
        public static IServiceCollection RegisterRulesServiceModule(this IServiceCollection services)
        {
            services.AddSingleton<IRulesService, RulesService>();
            return services;
        }
    }
}