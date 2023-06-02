using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.SharedServices;

public interface ISharedService
{
    Task<IEnumerable<SettingProfile>> GetSettingProfiles();

    Task<IEnumerable<int>> GetAllSettingTypes();
}

public class SharedService : ISharedService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    private readonly ICacheService cacheService;

    public SharedService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings.Value;
        this.cacheService = cacheService ?? new CacheService();
    }

    public async Task<IEnumerable<SettingProfile>> GetSettingProfiles()
        => await cacheService.GetOrSetAsync("CreateCustomerSettingProfile",
          () => this.GetSettingProfile(),
           new CacheItemOptions());

    public async Task<IEnumerable<int>> GetAllSettingTypes()
     => (await this.GetSettingProfile()).Select(i => i.SettingType ).Distinct();

    private async Task<IEnumerable<SettingProfile>> GetSettingProfile() => await dynamicRepository.FetchAsync<SettingProfile>(new GetSettingProfileCriteria { CreationStatus = appSettings.CreationStatus });
}
