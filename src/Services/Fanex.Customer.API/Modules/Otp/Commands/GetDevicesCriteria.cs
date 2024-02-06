namespace Fanex.Customer.API.Modules.Otp.Commands
{
    using Fanex.Data.Repository;

    public class GetDevicesCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public override string GetSettingKey() => "Age_OTP_CustomerOTPDeviceInfoSel";

        public override bool IsValid() => CustId > 0 && SubAccId >= 0;
    }
}