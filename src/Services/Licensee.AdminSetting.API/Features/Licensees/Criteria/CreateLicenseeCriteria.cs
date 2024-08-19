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
        string siteName)
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
            siteName)
    {
        public bool IsChinaView { get; } = isChinaView;

        public byte SpecialPlatform { get; } = specialPlatform;

        public string ProductList { get; set; }

        public byte Mode => CreateMode.NewLicensee;
    }
}