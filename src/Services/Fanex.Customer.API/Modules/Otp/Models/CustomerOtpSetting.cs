namespace Fanex.Customer.API.Modules.Otp.Models;

public class CustomerOtpSetting
{
    public CustomerOtpSetting(bool isEnableSophosOtp, bool isEnableTelegramOtp)
    {
        IsEnableSophosOtp = isEnableSophosOtp;
        IsEnableTelegramOtp = isEnableTelegramOtp;
    }

    public bool IsEnableSophosOtp { get; }

    public bool IsEnableTelegramOtp { get; }
}