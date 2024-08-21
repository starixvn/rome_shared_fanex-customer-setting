namespace Fanex.Customer.API.Shared.Utils;

public static class SPNameUtils
{
    public static string GetSPNameWithPrefix(string spName, int sitedbgroupid)
        => sitedbgroupid == 0 ? spName : $"DBSite{sitedbgroupid}_{spName}";
}
