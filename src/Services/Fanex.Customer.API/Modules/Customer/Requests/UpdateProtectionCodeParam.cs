namespace Fanex.Customer.API.Modules.Customer.Requests
{
    public class UpdateProtectionCodeParam
    {
        public UpdateProtectionCodeParam(
            string protectionCode,
            string hashKey,
            int custId,
            int subAccId,
            string langKey,
            string ip)
        {
            ProtectionCode = protectionCode;
            HashKey = hashKey;
            CustId = custId;
            SubAccId = subAccId;
            LangKey = langKey;
            IP = ip;
        }

        public string ProtectionCode { get; }

        public string HashKey { get; }

        public int CustId { get; }

        public int SubAccId { get; }

        public string LangKey { get; }

        public string IP { get; }
    }
}