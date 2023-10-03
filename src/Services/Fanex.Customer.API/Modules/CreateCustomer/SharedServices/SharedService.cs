using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.SharedServices;

public interface ISharedService
{
    Task<IEnumerable<SettingProfile>> GetSettingProfiles(string site, int currency);

    Task<IEnumerable<int>> GetAllSettingTypes(string site, int currency);
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

    public async Task<IEnumerable<SettingProfile>> GetSettingProfiles(string site, int currency)
        => await cacheService.GetOrSetAsync($"CreateCustomerSettingProfile_{site}_{currency}",
          () => this.GetSettingProfile(site, currency),
           new CacheItemOptions(){ RelativeExpiration = new TimeSpan(CacheDurationInHour, 0, 0) });

    public async Task<IEnumerable<int>> GetAllSettingTypes(string site, int currency)
        => (await this.GetSettingProfile(site, currency)).Select(i => i.SettingType).Distinct();

    private async Task<IEnumerable<SettingProfile>> GetSettingProfile(string site, int currency)
        => await dynamicRepository.FetchAsync<SettingProfile>(
            new GetSettingProfileCriteria(appSettings.CreationStatus, site, currency));
}
