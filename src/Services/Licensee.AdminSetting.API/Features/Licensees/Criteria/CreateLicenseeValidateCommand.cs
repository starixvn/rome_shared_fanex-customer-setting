using Fanex.Data;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class CreateLicenseeValidateCommand(
        int custDBGroupId,
        string productBetSetting,
        string productPT,
        string productStakeLevel,
        string siteName,
        byte mode) : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public int CustDBGroupId { get; } = custDBGroupId;

        public string ProductBetSetting { get; } = productBetSetting;

        public string ProductPT { get; } = productPT;

        public string ProductStakeLevel { get; } = productStakeLevel;

        public string SiteName { get; } = siteName;

        public byte Mode { get; } = mode;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        [SpParam(SqlParamName = "Usernames_Register")]
        public string RegisterUsernames { get; set; }

        public override string GetSettingKey() => SiteDBHelpers.GetSPNameInSiteDB(CustDBGroupId, "API_SiteBuilder_Validate");

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(ProductPT)
            && !string.IsNullOrWhiteSpace(ProductStakeLevel)
            && !string.IsNullOrWhiteSpace(ProductBetSetting)
            && !string.IsNullOrWhiteSpace(SiteName);
    }
}