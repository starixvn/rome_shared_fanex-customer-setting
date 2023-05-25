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
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.Resources;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<CustomerSetting> Query(QueryRequest request);

    Task<Result> Create(CreateRequest request);

    Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language);
}



public class CreateCustomerService : ICreateCustomerService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly AppSettings appSettings;
    private readonly ISharedService sharedService;
    private readonly ICacheRefService cacheRefService;
    private readonly Dictionary<int, dynamic> groupItemMapping;

    public CreateCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ISharedService sharedService, ICacheRefService cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.sharedService = sharedService;
        this.cacheRefService = cacheRefService;

        this.groupItemMapping = new()
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

    public async Task<CustomerSetting> Query(QueryRequest request)
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
                .GroupBy(o => new { o.GroupType, o.DisplayType })
                .Select(item => GetGroupType(request.UplineCustId, productGroup.Key, item.Key.GroupType, item.Key.DisplayType, request.Language).Result);
        }

        GetNameFromRemotingRef(customerSetting, request.Language);
        return customerSetting;
    }

    public async Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language)
    {
        var settingProfile = await sharedService.GetSettingProfiles();
        var generalInfo = await dynamicRepository.GetAsync<GeneralInformation>(
                    new GetCustomerCriteria
                    {
                        CustId = custId,
                        CreationStatus = appSettings.CreationStatus
                    });

        var productGroups = settingProfile.Where(p => p.GroupType == 153 || p.GroupType == 160 && (appSettings.IsY7(generalInfo.SiteLabel) || p.GroupId != 4)).GroupBy(p => p.GroupId)
            .Select((g) => new ProductGroup
            {
                Id = g.Key,
                GroupTypes = g.GroupBy(t => new { t.GroupType, t.DisplayType }).Select(t => t).ToAsyncEnumerable().SelectAwait(async t =>
                {
                    var groupItems = await groupItemMapping[t.Key.GroupType].Query(custId, g.Key, t.Key.GroupType);
                    var groupType = new GroupType
                    {
                        Id = t.Key.GroupType,
                        Name = Labels.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(language)),
                        DisplayType = t.Key.DisplayType,
                        SubGroups = t.GroupBy(sb => sb.SubGroupId).Select(sb => new SubGroup
                        {
                            Id = sb.Key,
                            Name = cacheRefService.GetSubGroupName(sb.Key, language),
                            GroupItems = ((IEnumerable<GroupItemDefault>)groupItems).Join(
                                t,
                                groupItems => new { SportType = groupItems.SporttypeId, BetType = groupItems.BettypeId },
                                groupItemsProfile => new { groupItemsProfile.SportType, groupItemsProfile.BetType },
                                (groupItems, groupItemsProfile) => groupItems).Select(groupItem =>
                                {
                                    groupItem.SporttypeName = cacheRefService.GetSportTypeName(groupItem.SporttypeId, language);
                                    groupItem.BettypeName = cacheRefService.GetBetTypeName(groupItem.BettypeId, language);

                                    return groupItem;
                                })
                        })
                    };

                    return groupType;
                }).ToEnumerable()
            }).Where(p => p.GroupTypes.Any(t => t.SubGroups.Any(r => r.GroupItems.Any())));

        var result = productGroups.ToList();
        GetNameFromRemotingRef(result, generalInfo.UplineRoleId, language);
        return result;
    }

    public async Task<Result> Create(CreateRequest request)
    {
        return new Result();
    }

    private async Task<GroupType> GetGroupType(
        int uplineCustId,
        int productGroupId,
        int groupTypeId,
        short displayType,
        string language)
    {
        var settingProfile = await sharedService.GetSettingProfiles();
        var groupType = new GroupType
        {
            Id = groupTypeId,
            Name = Labels.ResourceManager.GetString($"GroupType{groupTypeId}", new System.Globalization.CultureInfo(language)),
            DisplayType = displayType
        };

        var groupItems = groupItemMapping[groupTypeId].Query(uplineCustId, productGroupId, groupTypeId).Result;
        groupType.SubGroups = settingProfile
                .Where(o => o.GroupId == productGroupId && o.GroupType == groupTypeId)
                .GroupBy(o => new { o.SubGroupId })
                .Select(o => new SubGroup { Id = o.Key.SubGroupId }).ToList();

        foreach (var subGroup in groupType.SubGroups)
        {
            subGroup.GroupItems = ((IEnumerable<GroupItemDefault>)groupItems).Join(
                settingProfile.Where(o => o.GroupId == productGroupId && o.GroupType == groupTypeId && (o.SubGroupId == subGroup.Id)),
                groupItems => new { SportType = groupItems.SporttypeId, BetType = groupItems.BettypeId },
                groupItemsProfile => new { groupItemsProfile.SportType, groupItemsProfile.BetType },
                (groupItems, groupItemsProfile) => groupItems).ToList();
        }

        return groupType;
    }

    private void GetNameFromRemotingRef(IEnumerable<ProductGroup> productGroups, int uplineRoleId, string language)
    {
        foreach (var productGroup in productGroups)
        {
            var groupTypes = new List<GroupType>();
            productGroup.Name = cacheRefService.GetProductGroupName(productGroup.Id, language);
            foreach (var groupType in productGroup.GroupTypes)
            {
                foreach (var subGroup in groupType.SubGroups)
                {
                    var groupItems = new List<GroupItemDefault>();
                    subGroup.Name = this.cacheRefService.GetSubGroupName(subGroup.Id, language);
                    foreach (var groupItem in subGroup.GroupItems)
                    {
                        GetDisplayNameByDisplayType(uplineRoleId, language, productGroup, groupType, groupItem);
                        groupItem.SporttypeName = this.cacheRefService.GetSportTypeName(groupItem.SporttypeId, language);
                        groupItem.BettypeName = this.cacheRefService.GetBetTypeName(groupItem.BettypeId, language);
                        groupItems.Add(groupItem);
                    }

                    subGroup.GroupItems = groupItems.ToList();
                }

                groupTypes.Add(groupType);
            }

            productGroup.GroupTypes = groupTypes;
        }
    }

    private void GetNameFromRemotingRef(CustomerSetting customerSetting, string language)
        => GetNameFromRemotingRef(
            customerSetting.ProductGroups.Select(productGroup => productGroup.Value).ToList(),
            customerSetting.GeneralInfo.UplineRoleId,
            language);

    private void GetDisplayNameByDisplayType(int uplineRoleId, string language, ProductGroup productGroup, GroupType groupType, GroupItemDefault groupItem)
    {
        if (groupType.DisplayType == (int)DisplayType.CommSB)
        {
            if (uplineRoleId == (int)CustomerLevel.Agent)
            {
                groupItem.DisplayName = this.cacheRefService.GetSportsbookCommMemberName(
                    productGroup.Id,
                    groupType.Id,
                    groupItem.SporttypeId,
                    groupItem.BettypeId,
                    language);
            }
            else
            {
                groupItem.DisplayName = this.cacheRefService.GetSportsbookCommSMAName(
                    productGroup.Id,
                    groupType.Id,
                    groupItem.SporttypeId,
                    groupItem.BettypeId,
                    language);
            }
        }
        else if (groupType.DisplayType == (int)DisplayType.G_GT_Sport_Bettype)
        {
            groupItem.DisplayName = this.cacheRefService.GetNameByGroupSubGroup(
                    productGroup.Id,
                    groupType.Id,
                    groupItem.SporttypeId,
                    groupItem.BettypeId,
                    language);
        }
    }
}