using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Otp.Criteria;

public class GetCustomerOtpCriteria : CriteriaBase
{
    public int CustId { get; set; }

    public int SubAccId { get; set; }

    public override string GetSettingKey() => "Age_OTP_CustomerOTPSel";

    public override bool IsValid() => CustId > 0 && SubAccId >= 0;
}