namespace Fanex.Customer.API.Modules.Otp.Params
{
    public class RemoveSecretKeyParam
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public DeviceInfo DeviceInfo { get; set; }

        public int ActorId { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public bool IsResetDownlineOrSubAccount { get; set; }
    }
}