
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Shared.Services;

public class GetServerDBConfigCriteria : CriteriaBase
{
    public string Site { get; }

    public GetServerDBConfigCriteria(string site)
    {
        Site = site;
    }

    public override string GetSettingKey() => "Acc_ServerDBConfigSel";

    public override bool IsValid() => true;
}
public class GetServerDBConfigByCustCriteria : CriteriaBase
{
    public int CustId { get; }

    public string UserName { get; }

    public GetServerDBConfigByCustCriteria(int custId, string userName)
    {
        CustId = custId;
        UserName = userName;
    }

    public override string GetSettingKey() => "Acc_ServerDBConfigSelByCust";

    public override bool IsValid() => CustId > 0 || !string.IsNullOrWhiteSpace(UserName);
}

public class ServerDBConfig
{
    public int SiteDBGroupID { get; set; }

    public string LinkedServerName { get; set; }

    public string ClientAccessName { get; set; }

    public string AGIP { get; set; }

    public int CustDBGroupID { get; set; }

    public string LinkedServerNameDBCUST { get; set; }

    public string ClientAccessNameDBCUST { get; set; }

    public string AGIP_DBCUST { get; set; }
}

public interface IScaleoutDBService
{
    Task<int> GetCustDBGroupIdBySite(string site);

    Task<IEnumerable<ServerDBConfig>> GetServerDBConfigBySite(string site);

    string GetSPNameWithPrefix(string spName, int sitedbgroupid); 
}

public class ScaleoutDBService : IScaleoutDBService
{
    private readonly IDynamicRepository dynamicRepository;

    private const int DefaultCustDBGroupID = 0;
    
    public ScaleoutDBService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<IEnumerable<ServerDBConfig>> GetServerDBConfigBySite(string site)
    => await dynamicRepository.FetchAsync<ServerDBConfig>(new GetServerDBConfigCriteria(site));

    public async Task<int> GetCustDBGroupIdBySite(string site)
    => (await GetServerDBConfigBySite(site)).ToList().DefaultIfEmpty(new ServerDBConfig() {CustDBGroupID = DefaultCustDBGroupID}).FirstOrDefault().CustDBGroupID;

    public async Task<IEnumerable<ServerDBConfig>> GetServerDBConfigByCust(string site)
    => await dynamicRepository.FetchAsync<ServerDBConfig>(new GetServerDBConfigCriteria(site));

    public string GetSPNameWithPrefix(string spName, int sitedbgroupid)
        => sitedbgroupid == 0 ? spName : $"SiteDB{sitedbgroupid}_{spName}";
}
