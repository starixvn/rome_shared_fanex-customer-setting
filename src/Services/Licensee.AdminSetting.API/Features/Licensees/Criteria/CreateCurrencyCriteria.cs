namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class CreateCurrencyCriteria(int custDBGroupId, string customers, string productBetSetting, string productPT, string productStakeLevel, string productComm, string creator, string ip, bool isCreateSub, string licenseeType, int siteId, string siteName)
        : CreateLicenseeCriteria(custDBGroupId, customers, productBetSetting, productPT, productStakeLevel, productComm, creator, ip, isCreateSub, licenseeType, false, 0, siteId, siteName)
    {
        public string CurrencyList { get; set; }

        public decimal PT_Sportsbook { get; set; }

        public override byte Mode => CreateMode.NewCurrency;
    }
}