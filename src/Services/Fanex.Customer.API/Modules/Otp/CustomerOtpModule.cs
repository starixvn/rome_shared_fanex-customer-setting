using Fanex.Logging;
using ONEbook.OTP.Helpers;
using ONEbook.OTP.Services;

namespace Fanex.Customer.API.Modules.Otp
{
    public static class CustomerOtpModule
    {
        public static void RegisterCustomerOtpService(this IServiceCollection services)
        {
            services.AddSingleton<Logging.ILogger, Logger>();
            services.AddSingleton<OtpService, OtpServiceImpl>();
            services.AddSingleton<OtpAdapter, OtpAdapterImpl>();
            services.AddSingleton<QrCodeAdapter, QrCodeAdapterImpl>();
            services.AddSingleton<RandomHelper, RandomHelperImpl>();
            services.AddSingleton<IJsonHelper, JsonHelper>();
            services.AddSingleton<ICustomerOtpService, CustomerOtpService>();
        }
    }
}