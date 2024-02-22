namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateProtectionCodeCriteria : NonQueryCommand
    {
        public UpdateProtectionCodeCriteria(
            string protectionCode,
            int custId,
            int subAccId,
            string langKey,
            string ip)
        {
            ProtectionCode = protectionCode;
            CustId = custId;
            SubAccId = subAccId;
            Language = langKey;
            IP = ip;
        }

        public string ProtectionCode { get; }

        public int CustId { get; }

        public int SubAccId { get; }

        public string Language { get; }

        public string IP { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "Age_CustomerUpdProtectionCode";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(ProtectionCode)
            && CustId > 0
            && SubAccId >= 0;
    }
}
