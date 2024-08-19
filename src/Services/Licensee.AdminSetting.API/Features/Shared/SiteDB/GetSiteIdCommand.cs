using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public class GetSiteIdCommand(string siteName) : NonQueryCommand
    {
        public string SiteName { get; } = siteName;

        public string SiteId { get; set; }

        public override string GetSettingKey() => "API_Dep_GetSiteID_LicVendorProfile";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(SiteName);
    }
}