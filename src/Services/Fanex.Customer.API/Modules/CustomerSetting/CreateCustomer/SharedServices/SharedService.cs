using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.SharedServices;

public class SharedService : ISharedService
{
    private const int CacheDurationInHour = 24;

    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    private readonly ICacheService cacheService;

    public enum ProfileMode
    {
        AddNew = 1,
        Edit = 2
    }

    public SharedService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings.Value;
        this.cacheService = cacheService ?? new CacheService();
    }

    public async Task<IEnumerable<SettingProfile>> GetSettingProfiles(byte systemId, string site, int currency, ProfileMode mode)
        => await cacheService.GetOrSetAsync($"CreateCustomerSettingProfile_{systemId}_{site}_{currency}_{mode}",
          () => this.GetSettingProfile(systemId, site, currency, mode),
           new CacheItemOptions() { RelativeExpiration = new TimeSpan(CacheDurationInHour, 0, 0) });

    public async Task<int?> GetSettingTypeByGroupType(byte systemId, string site, int currency, int groupTypeId, ProfileMode mode)
            => (await GetSettingProfiles(systemId, site, currency, mode)).FirstOrDefault(g => g.GroupType == groupTypeId)?.SettingType;

    public async Task<IEnumerable<int>> GetAllSettingTypes(byte systemId, string site, int currency, ProfileMode mode)
        => (await this.GetSettingProfile(systemId, site, currency, mode)).Select(i => i.SettingType).Distinct();

    private async Task<IEnumerable<SettingProfile>> GetSettingProfile(byte systemId, string site, int currency, ProfileMode mode)
        => await dynamicRepository.FetchAsync<SettingProfile>(
            new GetSettingProfileCriteria(appSettings.SettingStatus, site, currency, systemId, (int)mode));
}
