namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateUsageLanguageCommand : NonQueryCommand
    {
        public UpdateUsageLanguageCommand(int custId, int subAccId, string languageKey, string ip)
        {
            CustId = custId;
            SubAccId = subAccId;
            LanguageUsage = languageKey;
            IP = ip;
        }

        public int CustId { get; }

        public int SubAccId { get; }

        public string LanguageUsage { get; }

        public string IP { get; }

        public override string GetSettingKey()
            => "Age_CustLanguageUsageUpd";

        public override bool IsValid()
            => CustId > 0 && SubAccId >= 0 && !string.IsNullOrWhiteSpace(LanguageUsage);
    }
}
