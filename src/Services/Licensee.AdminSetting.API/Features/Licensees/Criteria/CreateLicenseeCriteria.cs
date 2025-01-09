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
        byte siteType,
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
        public bool IsChinaView { get; } = isChinaView;

        public byte SpecialPlatform { get; } = specialPlatform;

        public byte SiteType { get; } = siteType;

        public string ProductList { get; set; }

        public byte Mode => CreateMode.NewLicensee;
    }
}