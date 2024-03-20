using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.SharedServices;

public interface ISharedService
{
    Task<IEnumerable<SettingProfile>> GetSettingProfiles(byte systemId, string site, int currency);

    Task<IEnumerable<int>> GetAllSettingTypes(byte systemId, string site, int currency);

    Task<int?> GetSettingTypeByGroupType(byte systemId, string site, int currency, int groupTypeId);
}

public class SharedService : ISharedService
{
    private const int CacheDurationInHour = 24;

    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    private readonly ICacheService cacheService;

    public SharedService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings.Value;
        this.cacheService = cacheService ?? new CacheService();
    }

    public async Task<IEnumerable<SettingProfile>> GetSettingProfiles(byte systemId, string site, int currency)
        => await cacheService.GetOrSetAsync($"CreateCustomerSettingProfile_{systemId}_{site}_{currency}",
          () => this.GetSettingProfile(systemId, site, currency),
           new CacheItemOptions() { RelativeExpiration = new TimeSpan(CacheDurationInHour, 0, 0) });

    public async Task<int?> GetSettingTypeByGroupType(byte systemId, string site, int currency, int groupTypeId)
            =>(await GetSettingProfiles(systemId, site, currency)).FirstOrDefault(g => g.GroupType == groupTypeId)?.SettingType;

    public async Task<IEnumerable<int>> GetAllSettingTypes(byte systemId, string site, int currency)
        => (await this.GetSettingProfile(systemId, site, currency)).Select(i => i.SettingType).Distinct();

    private async Task<IEnumerable<SettingProfile>> GetSettingProfile(byte systemId, string site, int currency)
        => await dynamicRepository.FetchAsync<SettingProfile>(
            new GetSettingProfileCriteria(appSettings.SettingStatus, site, currency, systemId));
}
