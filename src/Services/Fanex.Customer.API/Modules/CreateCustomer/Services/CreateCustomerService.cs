using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<CustomerSetting> Query(int uplineCustId);

}

public class CreateCustomerService : ICreateCustomerService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    private readonly ICacheService cacheService;

    public CreateCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.cacheService = cacheService;
    }
    public async Task<CustomerSetting> Query(int uplineCustId)
    {
        var settingProfile = (await cacheService.GetOrSetAsync("CreateCustomerSettingProfile",
          () => GetSettingProfile(),
           new CacheItemOptions())).ToList();

        var customerSetting = new CustomerSetting
        {
            GeneralInfo = await dynamicRepository.GetAsync<GeneralInformation>(
                    new GetCustomerCriteria
                    {
                        CustId = uplineCustId,
                        CreationStatus = appSettings.CreationStatus
                    }),
        };

        customerSetting.ProductGroups = new Dictionary<int, ProductGroup>();
        var productGroups = settingProfile.GroupBy(o => o.GroupId).Select(group => new ProductGroup { ProductGroupId = group.Key }).ToList();
        foreach (var productGroup in productGroups)
        {
            customerSetting.ProductGroups.Add(productGroup.ProductGroupId, productGroup);
        }



        foreach (var productGroup in customerSetting.ProductGroups)
        {
            productGroup.Value.GroupTypes = settingProfile.Where(o => o.GroupId == productGroup.Key).GroupBy(o => o.GroupType)
                .Select(item => new GroupType
                {
                    GroupTypeId = item.Key
                }).ToList();

        }

        return customerSetting;
    }

    private static async Task<ProductGroup> GetProductGroup(int GroupId)
    {
        return new ProductGroup();
    }

    private static async Task<GroupType> GetGroupType()
    {
        return new GroupType();
    }

    private async Task<IEnumerable<SettingProfile>> GetSettingProfile() => await dynamicRepository.FetchAsync<SettingProfile>(new GetSettingProfileCriteria { CreationStatus = appSettings.CreationStatus });
}