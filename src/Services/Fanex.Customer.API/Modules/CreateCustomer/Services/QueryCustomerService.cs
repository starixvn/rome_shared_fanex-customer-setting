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
        : base(dynamicRepository, appSettings, sharedService, cacheRefService)
    {
    }

    public async Task<CustomerSetting> Query(QueryRequest request)
    {
        var settingProfile = await sharedService.GetSettingProfiles();
        var generalInfo = await dynamicRepository.GetAsync<GeneralInformation>(
                    new GetCustomerCriteria
                    {
                        CustId = request.UplineCustId,
                        CreationStatus = appSettings.CreationStatus
                    });
        var customerSetting = new CustomerSetting
        {
            GeneralInfo = generalInfo,
            ProductGroups = settingProfile.Where(p => appSettings.IsY7(generalInfo.SiteLabel) || p.GroupId != 4).GroupBy(p => p.GroupId)
                .Select((g) => new ProductGroup
                {
                    Id = g.Key,
                    Name = cacheRefService.GetProductGroupName(g.Key, request.Language),
                    GroupTypes = g.GroupBy(t => new { t.GroupType, t.DisplayType }).Select(t => t).ToAsyncEnumerable().SelectAwait(async t =>
                    {
                        var groupItems = await groupItemMapping[t.Key.GroupType].Query(request.UplineCustId, g.Key, t.Key.GroupType);
                        var groupType = new GroupType
                        {
                            Id = t.Key.GroupType,
                            Name = Labels.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
                            DisplayType = t.Key.DisplayType,
                            GroupItems =
                            ((IEnumerable<GroupItemDefault>)groupItems).Join(
                                t.GroupBy(sb => new { sb.SubGroupId, sb.SportType, sb.BetType }),
                                groupItems => new { SportType = groupItems.SportTypeId, BetType = groupItems.BetTypeId },
                                groupItemsProfile => new { groupItemsProfile.Key.SportType, groupItemsProfile.Key.BetType },
                                (groupItem, groupItemsProfile) =>
                                {
                                    groupItem.SubGroupId = groupItemsProfile.Key.SubGroupId;
                                    groupItem.SubGroupName = this.cacheRefService.GetSubGroupName(groupItemsProfile.Key.SubGroupId, request.Language);
                                    groupItem.SportTypeName = cacheRefService.GetSportTypeName(groupItem.SportTypeId, request.Language);
                                    groupItem.BetTypeName = cacheRefService.GetBetTypeName(groupItem.BetTypeId, request.Language);
                                    groupItem.DisplayName = GetDisplayNameByDisplayType(
                                        groupItem.SportTypeId,
                                        groupItem.BetTypeId,
                                        g.Key,
                                        t.Key.GroupType,
                                        t.Key.DisplayType,
                                        request.UplineRoleId,
                                        request.Language);

                                    return groupItem;
                                })
                        };
                        return groupType;
                    }).ToEnumerable()
                }).Where(p => p.GroupTypes.Any())
        };

        customerSetting.GeneralInfo.UplineRoleId = request.UplineRoleId;
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

    private string GetDisplayNameByDisplayType(
        int sportTypeId,
        int betTypeId,
        int productGroupId,
        int groupTypeId,
        int displayType,
        int uplineRoleId,
        string language)
    {
        if (displayType == (int)DisplayType.CommSB)
        {
            if (uplineRoleId == (int)CustomerLevel.Agent)
            {
                return this.cacheRefService.GetSportsbookCommMemberName(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
            }
            else
            {
                return this.cacheRefService.GetSportsbookCommSMAName(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
            }
        }
        else if (displayType == (int)DisplayType.G_GT_Sport_Bettype)
        {
            return this.cacheRefService.GetNameByGroupSubGroup(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
        }

        return string.Empty;
    }
}