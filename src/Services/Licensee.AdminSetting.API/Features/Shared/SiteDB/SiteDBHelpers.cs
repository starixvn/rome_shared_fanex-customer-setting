namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public static class SiteDBHelpers
    {
        public static string GetSPNameInSiteDB(int siteDBGroupId, string spName)
            => siteDBGroupId == 0 ? spName : $"SiteDB{siteDBGroupId}_{spName}";
    }
}