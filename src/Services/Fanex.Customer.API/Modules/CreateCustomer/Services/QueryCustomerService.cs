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

public interface IQueryCustomerService
{
    Task<CustomerSetting> Query(QueryRequest request);

    Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language);
}

public class QueryCustomerService : BaseCustomerService, IQueryCustomerService
{
    public QueryCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ISharedService sharedService, ICacheRefService cacheRefService)
        :base(dynamicRepository, appSettings,sharedService,cacheRefService)
    {
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
                Name = cacheRefService.GetProductGroupName(g.Key, language),
                GroupTypes = g.GroupBy(t => new { t.GroupType, t.DisplayType }).Select(t => t).ToAsyncEnumerable().SelectAwait(async t =>
                {
                    var groupItems = await groupItemMapping[t.Key.GroupType].Query(custId, g.Key, t.Key.GroupType);
                    var groupType = new GroupType
                    {
                        Id = t.Key.GroupType,
                        Name = Labels.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(language)),
                        DisplayType = t.Key.DisplayType,
                         GroupItems = 
                        ((IEnumerable<GroupItemDefault>)groupItems).Join(
                            t.GroupBy(sb => new { sb.SubGroupId, sb.SportType, sb.BetType }),
                            groupItems => new { SportType = groupItems.SportTypeId, BetType = groupItems.BetTypeId },
                            groupItemsProfile => new { groupItemsProfile.Key.SportType, groupItemsProfile.Key.BetType },
                            (groupItem, groupItemsProfile) =>
                            {
                                groupItem.SubGroupId = groupItemsProfile.Key.SubGroupId;
                                groupItem.SubGroupName = this.cacheRefService.GetSubGroupName(groupItemsProfile.Key.SubGroupId, language);
                                groupItem.SportTypeName = cacheRefService.GetSportTypeName(groupItem.SportTypeId, language);
                                groupItem.BetTypeName = cacheRefService.GetBetTypeName(groupItem.BetTypeId, language);

                                return groupItem;
                            })
                    };

                    return groupType;
                }).ToEnumerable()
            }).Where(p => p.GroupTypes.Any());

        return productGroups;
    }

    private async Task<GroupType> GetGroupType(
        int uplineCustId,
        int productGroupId,
        int groupTypeId,
        short displayType,
        string language)
        => new GroupType
        {
            Id = groupTypeId,
            Name = Labels.ResourceManager.GetString($"GroupType{groupTypeId}", new System.Globalization.CultureInfo(language)),
            DisplayType = displayType,
            GroupItems = await groupItemMapping[groupTypeId].Query(uplineCustId, productGroupId, groupTypeId)
        };

    private void GetNameFromRemotingRef(IEnumerable<ProductGroup> productGroups, int uplineRoleId, string language)
    {
        foreach (var productGroup in productGroups)
        {
            var groupTypes = new List<GroupType>();
            productGroup.Name = cacheRefService.GetProductGroupName(productGroup.Id, language);
            foreach (var groupType in productGroup.GroupTypes)
            {
                var groupItems = new List<GroupItemDefault>();
                foreach (var groupItem in groupType.GroupItems)
                {
                    groupItem.SubGroupName = this.cacheRefService.GetSubGroupName(groupItem.SubGroupId, language);
                    GetDisplayNameByDisplayType(uplineRoleId, language, productGroup, groupType, groupItem);
                    groupItem.SportTypeName = this.cacheRefService.GetSportTypeName(groupItem.SportTypeId, language);
                    groupItem.BetTypeName = this.cacheRefService.GetBetTypeName(groupItem.BetTypeId, language);
                    groupItems.Add(groupItem);
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
                    groupItem.SportTypeId,
                    groupItem.BetTypeId,
                    language);
            }
            else
            {
                groupItem.DisplayName = this.cacheRefService.GetSportsbookCommSMAName(
                    productGroup.Id,
                    groupType.Id,
                    groupItem.SportTypeId,
                    groupItem.BetTypeId,
                    language);
            }
        }
        else if (groupType.DisplayType == (int)DisplayType.G_GT_Sport_Bettype)
        {
            groupItem.DisplayName = this.cacheRefService.GetNameByGroupSubGroup(
                    productGroup.Id,
                    groupType.Id,
                    groupItem.SportTypeId,
                    groupItem.BetTypeId,
                    language);
        }
    }
}