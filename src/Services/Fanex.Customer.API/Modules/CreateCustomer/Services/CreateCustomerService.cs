using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Data.Repository;
using Fanex.RemotingRef;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<CustomerSetting> Query(int uplineCustId, int uplineRoleId);

    Task<CustomerSetting> QueryPTTemplate(int custId, int roleId);

    Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language = "en-US");
}

public class CreateCustomerService : ICreateCustomerService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    private readonly ICacheService cacheService;

    private readonly IRemotingRefService remotingRef;

    public CreateCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ICacheService cacheService, IRemotingRefService remotingRef)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.cacheService = cacheService;
        this.remotingRef = remotingRef;
    }

    public async Task<CustomerSetting> Query(int uplineCustId, int uplineRoleId)
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
            ProductGroups = new Dictionary<int, ProductGroup>()
        };

        customerSetting.GeneralInfo.UplineRoleId = uplineRoleId;
        var productGroups = settingProfile.GroupBy(o => o.GroupId).Select(group => new ProductGroup { Id = group.Key }).ToList();
        foreach (var productGroup in productGroups)
        {
            customerSetting.ProductGroups.Add(productGroup.Id, productGroup);
        }

        foreach (var productGroup in customerSetting.ProductGroups)
        {

            productGroup.Value.GroupTypes = settingProfile
                .Where(o => o.GroupId == productGroup.Key)
                .GroupBy(o => new { o.GroupType, o.SubGroupId })
                .Select(item => GetGroupType(uplineCustId, productGroup.Key, item.Key.GroupType, item.Key.SubGroupId).Result);
        }

        GetNameFromRemotingRef(customerSetting);
        return customerSetting;
    }

    public async Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language)
    {
        var settingProfile = (await cacheService.GetOrSetAsync("CreateCustomerSettingProfile",
         () => GetSettingProfile(),
          new CacheItemOptions())).ToList();

        var productGroups = settingProfile.Where(p => p.GroupType == 153 || p.GroupType == 160).GroupBy(p => p.GroupId)
            .Select((g) => new ProductGroup
            {
                Id = g.Key,
                Name = remotingRef.GetResourceDataByRefIdAndResourceId("ProductGroup", g.Key.ToString(), language, string.Empty),
                GroupTypes = g.GroupBy(t => new { t.GroupType, t.SubGroupId }).Select(t => t).ToAsyncEnumerable().SelectAwait(async t =>
                {
                    var groupType = await GetGroupType(custId, g.Key, t.Key.GroupType, t.Key.SubGroupId);
                    groupType.SubGroupName = remotingRef.GetResourceDataByRefIdAndResourceId("SubGroup", groupType.SubGroupId.ToString(), "en-US", string.Empty);
                    return groupType;
                }).ToEnumerable()
            });

        return productGroups;
    }

    public async Task<CustomerSetting> QueryPTTemplate(int uplineCustId, int uplineRoleId)
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
            ProductGroups = new Dictionary<int, ProductGroup>()
        };

        customerSetting.GeneralInfo.UplineRoleId = uplineRoleId;
        var productGroups = settingProfile.GroupBy(o => o.GroupId).Select(group => new ProductGroup { Id = group.Key }).ToList();
        foreach (var productGroup in productGroups)
        {
            customerSetting.ProductGroups.Add(productGroup.Id, productGroup);
        }

        foreach (var productGroup in customerSetting.ProductGroups)
        {
            var groupTypes = settingProfile
                .Where(o => o.GroupId == productGroup.Key && (o.GroupType == 153 || o.GroupType == 160))
                .GroupBy(o => new { o.GroupType, o.SubGroupId })
                .Select(async item => await GetGroupType(uplineCustId, productGroup.Key, item.Key.GroupType, item.Key.SubGroupId));

            productGroup.Value.GroupTypes = settingProfile
                .Where(o => o.GroupId == productGroup.Key && (o.GroupType == 153 || o.GroupType == 160))
                .GroupBy(o => new { o.GroupType, o.SubGroupId })
                .Select(item => GetGroupType(uplineCustId, productGroup.Key, item.Key.GroupType, item.Key.SubGroupId).Result);
        }

        GetNameFromRemotingRef(customerSetting);
        return customerSetting;
    }

    private async Task<GroupType> GetGroupType(int uplineCustId, int productGroupId, int groupTypeId, int? subGroupId)
    {
        var groupType = new GroupType { Id = groupTypeId, SubGroupId = subGroupId };
        var service = (IGroupTypeBaseService)Activator.CreateInstance(Type.GetType($"Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType{groupTypeId}.GroupType{groupTypeId}Service, Fanex.Customer.API"), dynamicRepository, appSettings);
        groupType.GroupItems = await service.Query(uplineCustId, productGroupId);

        return groupType;
    }

    private void GetNameFromRemotingRef(CustomerSetting customerSetting)
    {
        foreach (var productGroup in customerSetting.ProductGroups)
        {
            var groupTypes = new List<GroupType>();
            productGroup.Value.Name = this.remotingRef.GetResourceDataByRefIdAndResourceId("ProductGroup", productGroup.Value.Id.ToString(), "en-US", productGroup.Value.Id.ToString());
            foreach (var groupType in productGroup.Value.GroupTypes)
            {
                groupType.SubGroupName = this.remotingRef.GetResourceDataByRefIdAndResourceId("SubGroup", groupType.SubGroupId?.ToString(), "en-US", groupType.Id.ToString());
                if (groupType.GroupItems != null)
                {
                    var groupItems = new List<GroupItemDefault>();
                    foreach (var groupItem in groupType.GroupItems)
                    {
                        string refkey = $"{productGroup.Value.Id}_{groupType.Id}_{groupItem.SporttypeId}_{groupItem.BettypeId}";
                        groupItem.DisplayName = this.remotingRef.GetResourceDataByRefIdAndResourceId("G_GT_Sport_Bettype", refkey, "en-US", refkey);
                        groupItem.SporttypeName = this.remotingRef.GetSportName(groupItem.SporttypeId.ToString(), "en-US");
                        groupItem.BettypeName = this.remotingRef.GetBetTypeName(groupItem.BettypeId.ToString(), "en-US");
                        groupItems.Add(groupItem);
                    }

                    groupType.GroupItems = groupItems.ToList();
                }
                groupTypes.Add(groupType);
            }

            productGroup.Value.GroupTypes = groupTypes;
        }
    }

    private async Task<IEnumerable<SettingProfile>> GetSettingProfile() => await dynamicRepository.FetchAsync<SettingProfile>(new GetSettingProfileCriteria { CreationStatus = appSettings.CreationStatus });
}