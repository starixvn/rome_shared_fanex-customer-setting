using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class InsertCurrencyCommand(string siteName, string currencyList, decimal ptSportsbook, string ip) : NonQueryCommand
    {
        public string SiteName { get; } = siteName;

        public string CurrencyList { get; } = currencyList;

        public decimal PT_Sportsbook { get; } = ptSportsbook;

        public string IP { get; } = ip;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int LicenseeId { get; set; }

        public override string GetSettingKey() => "API_SiteBuilder_LicenseeDB_CurrencyIns";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(SiteName) && !string.IsNullOrWhiteSpace(CurrencyList) && PT_Sportsbook > 0;
    }
}