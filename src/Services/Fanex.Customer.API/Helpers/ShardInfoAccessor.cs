namespace Fanex.Customer.API.Helpers;

public interface IShardInfoAccessor
{
    bool TryUseShardDB(string key, out string dbServer);
}

public class ShardInfoAccessor : IShardInfoAccessor
{
    const string PrefixSiteDB = "sitedb_";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShardInfoAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public bool TryUseShardDB(string key, out string dbServer)
    {
        if (IsBelongSiteDB(key))
        {
            dbServer = SiteDBServer;
            return true;
        }
        dbServer = string.Empty;

        return false;
    }

    private static bool IsBelongSiteDB(string key) => key.StartsWith(PrefixSiteDB, StringComparison.OrdinalIgnoreCase);

    private string SiteDBServer => _httpContextAccessor.HttpContext.Request.Headers["X-SiteDBServer"].ToString();
}

