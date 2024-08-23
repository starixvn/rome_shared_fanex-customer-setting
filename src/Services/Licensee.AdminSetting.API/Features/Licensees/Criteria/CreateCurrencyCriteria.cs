namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class CreateCurrencyCriteria(
        int custDBGroupId,
        string customers,
        string productBetSetting,
        string productPT,
        string productStakeLevel,
        string productComm,
        string creator,
        string ip,
        bool isCreateSub,
        string licenseeType,
        int siteId,
        string siteName,
        byte settingStatus)
        : SiteBuilderBaseCriteria(
            custDBGroupId,
            customers,
            productBetSetting,
            productPT,
            productStakeLevel,
            productComm,
            creator,
            ip,
            isCreateSub,
            licenseeType,
            siteId,
            siteName,
            settingStatus)
    {
        public string CurrencyList { get; set; }

        public decimal PT_Sportsbook { get; set; }

        public byte Mode => CreateMode.NewCurrency;
    }
}