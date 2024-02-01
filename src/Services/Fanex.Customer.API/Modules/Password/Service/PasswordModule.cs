using Fanex.Cryptography.Aes;
using TelegramBot.Client.Services;

namespace Fanex.Customer.API.Modules.Password.Service
{
    public static class PasswordModule
    {
        public static void RegisterPasswordService(this IServiceCollection services)
        {
            services
                .AddSingleton<IAesService, AesService>()
                .AddSingleton<IEncryption, Encryption>()
                .AddSingleton<INotificationService, NotificationService>()
                .AddSingleton<IPasswordService, PasswordService>()
                .AddSingleton<ITelegramBotService, TelegramBotService>()
                .AddSingleton<IKickoutCustomerService, KickoutCustomerService>();
        }
    }
}