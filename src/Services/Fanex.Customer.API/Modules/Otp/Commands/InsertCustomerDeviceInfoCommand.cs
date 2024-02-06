namespace Fanex.Customer.API.Modules.Otp.Commands
{
    using Fanex.Data.Repository;

    public class InsertCustomerDeviceInfoCommand : NonQueryCommand
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string DeviceId { get; set; }

        public string DeviceInfo { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Age_OTP_CustomerOTPDeviceInfoIns";
        }

        public override bool IsValid()
        {
            return CustId > 0
                && SubAccId >= 0
                && !string.IsNullOrWhiteSpace(Language)
                && !string.IsNullOrWhiteSpace(IP);
        }
    }
}