namespace Fanex.Customer.API.Modules.Otp.Commands
{
    using Fanex.Data.Repository;

    public class CheckEnableOtpCommand : NonQueryCommand
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public bool Enable { get; set; }

        public override string GetSettingKey()
        {
            return "Age_OTP_CustomerCheckEnableOTP";
        }

        public override bool IsValid()
        {
            return CustId > 0
                && SubAccId >= 0;
        }
    }
}