using Fanex.Data;
using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class CreateLicenseeValidateCommand(
        int custDBGroupId,
        string productBetSetting,
        string productPT,
        string productStakeLevel,
        string siteName) : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public int CustDBGroupId { get; } = custDBGroupId;

        public string ProductBetSetting { get; } = productBetSetting;

        public string ProductPT { get; } = productPT;

        public string ProductStakeLevel { get; } = productStakeLevel;

        public string SiteName { get; } = siteName;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        [SpParam(SqlParamName = "CustList_Register")]
        public string Customers { get; set; }

        public override string GetSettingKey()
        {
            if (CustDBGroupId > 0)
            {
                return $"SiteDB{CustDBGroupId}_API_SiteBuilder_Validate";
            }

            return "API_SiteBuilder_Validate";
        }

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(ProductPT)
            && !string.IsNullOrWhiteSpace(ProductStakeLevel)
            && !string.IsNullOrWhiteSpace(ProductBetSetting)
            && !string.IsNullOrWhiteSpace(SiteName);
    }
}