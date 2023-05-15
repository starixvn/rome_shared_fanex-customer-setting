using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.SharedServices;

public interface ISharedService
{
    Task<IEnumerable<SettingProfile>> GetSettingProfile();
}

public class SharedService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    private readonly ICacheService cacheService;

    public SharedService(IDynamicRepository dynamicRepository, AppSettings appSettings, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository?? new DynamicRepository();
        this.appSettings = appSettings;
        this.cacheService = cacheService?? new CacheService();
    }

    internal async Task<IEnumerable<SettingProfile>> GetSettingProfile()
        => await cacheService.GetOrSetAsync("CreateCustomerSettingProfile",
          () => dynamicRepository.FetchAsync<SettingProfile>(new GetSettingProfileCriteria { CreationStatus = appSettings.CreationStatus }));

}
