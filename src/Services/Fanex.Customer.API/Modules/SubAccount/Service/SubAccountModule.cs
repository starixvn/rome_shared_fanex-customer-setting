namespace Fanex.Customer.API.Modules.SubAccount.Service
{
    public static class SubAccountModule
    {
        public static void RegisterSubAccountService(this IServiceCollection services)
        {
            services.AddHttpClient<ISubAccountService, SubAccountService>();
        }
    }
}