using Fanex.Data;
using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class RegisterCustomersCommand(
        string registerUsernames,
        string siteName,
        bool isChinaView,
        byte specialPlatform,
        string productStakeLevel,
        string ip,
        byte mode,
        int siteDBGroupId) : NonQueryCommand
    {
        public string RegisterUsernames { get; } = registerUsernames;

        public string SiteName { get; } = siteName;

        public bool IsChinaView { get; } = isChinaView;

        public byte SpecialPlatform { get; } = specialPlatform;

        public string ProductStakeLevel { get; } = productStakeLevel;

        public string IP { get; } = ip;

        public byte Mode { get; } = mode;

        public int SiteDBGroupId { get; } = siteDBGroupId;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public int SiteId { get; set; }

        [SpParam(SqlParamName = "Customers_List")]
        public string Customers { get; set; }

        public string FailedCurrencies { get; set; }

        public override string GetSettingKey() => "API_SiteBuilder_Customers_Register";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(RegisterUsernames);
    }
}