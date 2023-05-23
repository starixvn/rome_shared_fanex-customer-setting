using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Otp;

public class UpdateCustomerStatusAfterVerifyOtpCommand : NonQueryCommand
{
    public int CustId { get; set; }

    public int SubAccId { get; set; }

    public bool CheckedOtp { get; set; }

    public string FunctionName { get; set; }

    public string Language { get; set; }

    public string IP { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_OTP_CustomerStatusUpdAfterVerifiedOTP";

    public override bool IsValid() => CustId > 0 && SubAccId >= 0 && !string.IsNullOrWhiteSpace(Language) && !string.IsNullOrWhiteSpace(IP);
}