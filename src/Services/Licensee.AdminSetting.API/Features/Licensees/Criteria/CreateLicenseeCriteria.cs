using Fanex.Data;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class CreateLicenseeCriteria(
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
        bool isChinaView,
        byte specialPlatform,
        int siteId,
        string siteName
    ) : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int CustDBGroupId { get; } = custDBGroupId;

        [SpParam(SqlParamName = "Customers_List")]
        public string Customers { get; } = customers;

        public string ProductBetSetting { get; } = productBetSetting;

        public string ProductPT { get; } = productPT;

        public string ProductStakeLevel { get; } = productStakeLevel;

        public string ProductComm { get; } = productComm;

        public string Creator { get; } = creator;

        public string IP { get; } = ip;

        public bool IsCreateSub { get; } = isCreateSub;

        public string LicenseeType { get; } = licenseeType;

        public bool IsChinaView { get; } = isChinaView;

        public byte SpecialPlatform { get; } = specialPlatform;

        public int SiteId { get; } = siteId;

        public string SiteName { get; } = siteName;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string InsBalExceptCustList { get; set; }

        public string FailedCurrencies { get; set; }

        public string ListSuperRollBack { get; set; }

        public virtual byte Mode => CreateMode.NewLicensee;

        public override string GetSettingKey() => SiteDBHelpers.GetSPNameInSiteDB(CustDBGroupId, "API_SiteBuilder_Licensee_Insert");

        public override bool IsValid() => !string.IsNullOrEmpty(Customers);
    }
}