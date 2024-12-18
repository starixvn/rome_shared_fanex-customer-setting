using Fanex.Cryptography.Aes;
using TelegramBot.Client.Services;

namespace Fanex.Customer.API.Modules.Password.Service
{
    public static class PasswordModule
    {
        public static void RegisterPasswordService(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services
                .AddSingleton<IAesService, AesService>()
                .AddSingleton<IEncryption, Encryption>()
                .AddSingleton<ITelegramBotService, TelegramBotService>()
                .AddSingleton<IKickoutCustomerService, KickoutCustomerService>()
                .AddHttpClient<INotificationService, NotificationService>(httpClient => httpClient.BaseAddress = new Uri(configurationManager["TelegramClientUrl"]));
                
            services
                .AddHttpClient<IPasswordService, PasswordService>();
        }
    }
}