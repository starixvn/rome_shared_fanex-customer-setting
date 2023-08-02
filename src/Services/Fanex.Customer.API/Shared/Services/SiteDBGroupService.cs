using Fanex.Caching;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Shared.Services;

public class GetSiteDBGroupCriteria : CriteriaBase
{
    public byte IsDeposit { get; } = 0;

    public override string GetSettingKey() => "Acc_CustSetting_SiteGroupSel";

    public override bool IsValid() => true;
}

public interface ISiteDBGroupService
{
    Task<IEnumerable<SiteDBGroup>> GetAll();

    Task<string> GetSiteDBName(string site);
}

public class SiteDBGroupService : ISiteDBGroupService
{
    private const string SiteDBGroupsCacheKey = "Fanex.Customer.API.Shared.Services.SiteDBGroupService.SiteDBGroups";
    private readonly IDynamicRepository repository;
    private readonly ICacheService cacheService;

    public SiteDBGroupService(IDynamicRepository repository, ICacheService cacheService)
    {
        this.repository = repository;
        this.cacheService = cacheService;
    }

    public Task<IEnumerable<SiteDBGroup>> GetAll()
        => cacheService.GetOrSetAsync(SiteDBGroupsCacheKey, () => repository.FetchAsync<SiteDBGroup>(new GetSiteDBGroupCriteria()));

    public async Task<string> GetSiteDBName(string site)
    {
        var dbGroups = await GetAll();
        return dbGroups.FirstOrDefault(g => g.Site.Equals(site, StringComparison.OrdinalIgnoreCase)).LinkedServerName;
    }
}