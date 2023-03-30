using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Otp.Criteria;

public class CheckEnableOtpCommand : NonQueryCommand
{
    public int CustId { get; set; }

    public int SubAccId { get; set; }

    public bool Enable { get; set; }

    public override string GetSettingKey() => "Age_OTP_CustomerCheckEnableOTP";

    public override bool IsValid() => CustId > 0 && SubAccId >= 0;
}