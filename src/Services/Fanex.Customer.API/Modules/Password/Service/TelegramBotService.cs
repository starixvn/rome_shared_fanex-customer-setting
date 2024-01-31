using TelegramBot.Client.Services;

namespace Fanex.Customer.API.Modules.Password.Service
{
    public interface ITelegramBotService
    {
        void NotifyChangePassword(int custId, int subAccId);

        Task NotifyChangePasswordAsync(int custId, int subAccId);

        void NotifyChangeSecurityCode(int custId, int subAccId);

        Task NotifyChangeSecurityCodeAsync(int custId, int subAccId);
    }

    public class TelegramBotService : ITelegramBotService
    {
        private readonly INotificationService notificationService;

        private readonly Fanex.Logging.ILogger logger;

        public TelegramBotService(INotificationService notificationService, Fanex.Logging.ILogger logger)
        {
            this.notificationService = notificationService;
            this.logger = logger;
        }

        public void NotifyChangePassword(int custId, int subAccId)
            => NotifyChangePasswordAsync(custId, subAccId).GetAwaiter().GetResult();

        public async Task NotifyChangePasswordAsync(int custId, int subAccId)
        {
            try
            {
                await notificationService.NotifyChangePassword(custId, subAccId);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }
        }

        public void NotifyChangeSecurityCode(int custId, int subAccId)
            => NotifyChangeSecurityCodeAsync(custId, subAccId).GetAwaiter().GetResult();

        public async Task NotifyChangeSecurityCodeAsync(int custId, int subAccId)
        {
            try
            {
                await notificationService.NotifyChangeSecurityCode(custId, subAccId);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }
        }
    }
}