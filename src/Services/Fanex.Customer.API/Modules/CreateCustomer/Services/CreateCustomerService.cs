using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType159;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<CustomerSetting> Query(CreateCustomerRequest request);

    Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language);
}



public class CreateCustomerService : ICreateCustomerService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly AppSettings appSettings;
    private readonly ISharedService sharedService;
    private readonly ICacheRefService cacheRefService;
    private readonly Dictionary<int, dynamic> gropItemMapping;

    public CreateCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ISharedService sharedService, ICacheRefService cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.sharedService = sharedService;
        this.cacheRefService = cacheRefService;

        this.gropItemMapping = new()
        {
            {150, new GroupTypeBaseService<CommissionGroup>(dynamicRepository, appSettings.Value)},
            {151, new GroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings.Value)},
            {152, new GroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings.Value)},
            {153, new GroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings.Value)},
            {154, new GroupTypeBaseService<ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings.Value)},
            {155, new GroupTypeBaseService<WinlossLimitSettingItem>(dynamicRepository, appSettings.Value)},
            {156, new GroupTypeBaseService<CommissionSettingItem>(dynamicRepository, appSettings.Value)},
            {157, new GroupTypeBaseService<LossLimitSettingItem>(dynamicRepository, appSettings.Value)},
            {158, new GroupTypeBaseService<ViewModels.GroupType158.CommissionSettingItem>(dynamicRepository, appSettings.Value)},
            {159, new GroupTypeBaseService<BetsettingItem>(dynamicRepository, appSettings.Value)},
            {160, new GroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings.Value)}
        };
    }

    public async Task<CustomerSetting> Query(CreateCustomerRequest request)
    {
        var settingProfile = await sharedService.GetSettingProfiles();

        var customerSetting = new CustomerSetting
        {
            GeneralInfo = await dynamicRepository.GetAsync<GeneralInformation>(
                    new GetCustomerCriteria
                    {
                        CustId = request.UplineCustId,
                        CreationStatus = appSettings.CreationStatus
                    }),
            ProductGroups = new Dictionary<int, ProductGroup>()
        };

        customerSetting.GeneralInfo.UplineRoleId = request.UplineRoleId;
        var productGroups = settingProfile
            .GroupBy(o => o.GroupId)
            .Select(group => new ProductGroup { Id = group.Key })
            .ToList();

        foreach (var productGroup in productGroups)
        {
            customerSetting.ProductGroups.Add(productGroup.Id, productGroup);
        }

        foreach (var productGroup in customerSetting.ProductGroups)
        {
            productGroup.Value.GroupTypes = settingProfile
                .Where(o => o.GroupId == productGroup.Key)
                .GroupBy(o => new { o.GroupType, o.SubGroupId, o.DisplayType })
                .Select(item => GetGroupType(request.UplineCustId, productGroup.Key, item.Key.GroupType, item.Key.DisplayType, item.Key.SubGroupId).Result);
        }

        GetNameFromRemotingRef(customerSetting, request.Language);
        return customerSetting;
    }

    public async Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language)
    {
        var settingProfile = await sharedService.GetSettingProfiles();

        var productGroups = settingProfile.Where(p => p.GroupType == 153 || p.GroupType == 160).GroupBy(p => p.GroupId)
            .Select((g) => new ProductGroup
            {
                Id = g.Key,
                Name = cacheRefService.GetProductGroupName(g.Key, language),
                GroupTypes = g.GroupBy(t => new { t.GroupType, t.SubGroupId, t.DisplayType }).Select(t => t).ToAsyncEnumerable().SelectAwait(async t =>
                {
                    var groupType = await GetGroupType(custId, g.Key, t.Key.GroupType, t.Key.DisplayType, t.Key.SubGroupId);
                    groupType.SubGroupName = cacheRefService.GetSubGroupName(groupType.SubGroupId, language);
                    return groupType;
                }).ToEnumerable()
            }).Where(p => p.GroupTypes.Any(t => t.GroupItems.Any()));

        return productGroups;
    }

    private async Task<GroupType> GetGroupType(int uplineCustId, int productGroupId, int groupTypeId, short displayType, int? subGroupId)
        => new GroupType
        {
            Id = groupTypeId,
            SubGroupId = subGroupId,
            DisplayType = displayType,
            GroupItems = await gropItemMapping[groupTypeId].Query(uplineCustId, productGroupId, groupTypeId)
        };

    private void GetNameFromRemotingRef(CustomerSetting customerSetting, string language)
    {
        foreach (var productGroup in customerSetting.ProductGroups)
        {
            var groupTypes = new List<GroupType>();
            productGroup.Value.Name = cacheRefService.GetProductGroupName(productGroup.Value.Id, language);
            foreach (var groupType in productGroup.Value.GroupTypes)
            {
                groupType.SubGroupName = this.cacheRefService.GetSubGroupName(groupType.SubGroupId, language);
                if (groupType.GroupItems != null)
                {
                    var groupItems = new List<GroupItemDefault>();
                    foreach (var groupItem in groupType.GroupItems)
                    {
                        GetDisplayNameByDisplayType(customerSetting, language, productGroup, groupType, groupItem);
                        groupItem.SporttypeName = this.cacheRefService.GetSporttypeName(groupItem.SporttypeId, language);
                        groupItem.BettypeName = this.cacheRefService.GetBettypeName(groupItem.BettypeId, language);
                        groupItems.Add(groupItem);
                    }

                    groupType.GroupItems = groupItems.ToList();
                }
                groupTypes.Add(groupType);
            }

            productGroup.Value.GroupTypes = groupTypes;
        }
    }

    private void GetDisplayNameByDisplayType(CustomerSetting customerSetting, string language, KeyValuePair<int, ProductGroup> productGroup, GroupType groupType, GroupItemDefault groupItem)
    {
        if (groupType.DisplayType == (int)DisplayType.CommSB)
        {
            if (customerSetting.GeneralInfo.UplineRoleId == (int)CustomerLevel.Agent)
            {
                groupItem.DisplayName = this.cacheRefService.GetSportsbookCommMemberName(
                    productGroup.Value.Id,
                    groupType.Id,
                    groupItem.SporttypeId,
                    groupItem.BettypeId,
                    language);
            }
            else
            {
                groupItem.DisplayName = this.cacheRefService.GetSportsbookCommSMAName(
                    productGroup.Value.Id,
                    groupType.Id,
                    groupItem.SporttypeId,
                    groupItem.BettypeId,
                    language);
            }
        }
        else if (groupType.DisplayType == (int)DisplayType.G_GT_Sport_Bettype)
        {
            groupItem.DisplayName = this.cacheRefService.GetNameByGroupSubGroup(
                    productGroup.Value.Id,
                    groupType.Id,
                    groupItem.SporttypeId,
                    groupItem.BettypeId,
                    language);
        }
    }
}