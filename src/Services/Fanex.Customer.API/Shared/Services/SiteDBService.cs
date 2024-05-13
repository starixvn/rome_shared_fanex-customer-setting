using Fanex.Data.Repository;

namespace Fanex.Customer.API.Shared.Services;

public class GetSiteDBConfig : CriteriaBase
{
    public GetSiteDBConfig(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override bool IsValid() => CustId > 0;

    public override string GetSettingKey() => "Acc_SiteDBConfigSel";
}

public class SiteDBConfig
{
    public int SiteDBGroupID { get; set; }

    public string LinkedServerName { get; set; }

    public string ClientAccessName { get; set; }

    public string AGIP { get; set; }

    public bool IsSiteDB => SiteDBGroupID != 0;
}

public interface ISiteDBService
{
    Task<SiteDBConfig> GetSiteDBConfigByCustId(int custId);
}

public class SiteDBService : ISiteDBService
{
    private readonly IDynamicRepository repository;

    public SiteDBService(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<SiteDBConfig> GetSiteDBConfigByCustId(int custId) 
        => await this.repository.GetAsync<SiteDBConfig>(new GetSiteDBConfig(custId));
}
