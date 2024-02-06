namespace Fanex.Customer.API.Modules.Otp.Commands
{
    using Fanex.Data.Repository;

    public class DeleteCustOtpSecretKeyCommand : NonQueryCommand
    {
        public int UserId { get; set; }

        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string DeviceInfo { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Age_OTP_CustomerOTPDel";
        }

        public override bool IsValid()
        {
            return CustId > 0
                && SubAccId >= 0
                && UserId > 0
                && !string.IsNullOrWhiteSpace(DeviceInfo)
                && !string.IsNullOrWhiteSpace(Language)
                && !string.IsNullOrWhiteSpace(IP);
        }
    }
}