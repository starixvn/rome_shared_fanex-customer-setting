using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CreateCustomer.Resources;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface IQueryCustomerService
{
    Task<CustomerSetting> Query(QueryRequest request);
}

public class QueryCustomerService : BaseCustomerService, IQueryCustomerService
{
    public QueryCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ISharedService sharedService, ICacheRefService cacheRefService, IMessageService messageService)
        : base(dynamicRepository, appSettings, sharedService, cacheRefService, messageService)
    {
    }

    public async Task<CustomerSetting> Query(QueryRequest request)
    {
        var settingProfile = await sharedService.GetSettingProfiles();
        var generalInfo = await GetGeneralInfo(request);
        var customerSetting = new CustomerSetting
        {
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(settingProfile, request)
        };

        customerSetting.GeneralInfo.UplineRoleId = request.UplineRoleId;
        return customerSetting;
    }

    private async Task<GeneralInformation> GetGeneralInfo(QueryRequest request)
    {
        return await dynamicRepository.GetAsync<GeneralInformation>(
        new GetCustomerCriteria { 
            CustId = request.UplineCustId, 
            RoleId = request.UplineRoleId, 
            CreationStatus = appSettings.CreationStatus 
            });
    }

    private IEnumerable<ProductGroup> GetProductGroups(IEnumerable<SettingProfile> settingProfile, QueryRequest request)
    {
        return settingProfile.GroupBy(p => p.GroupId)
        .Select((g) => new ProductGroup
        {
            Id = g.Key,
            Name = cacheRefService.GetProductGroupName(g.Key, request.Language),
            GroupTypes = GetGroupTypes(g, request)
        }).Where(p => p.GroupTypes.Any());
    }

    private IEnumerable<GroupType> GetGroupTypes(IGrouping<int, SettingProfile> g, QueryRequest request)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = IsPTGroupType(t.Key.GroupType) ? GetPTGroupTypeByCustomerLevel(t.Key.GroupType, request.UplineRoleId - 1) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.Query(request.UplineCustId, request.UplineRoleId, g.Key, t.Key.GroupType);
                var groupType = new GroupType
                {
                    Id = t.Key.GroupType,
                    Name = Labels.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
                    DisplayType = t.Key.DisplayType,
                    GroupItems = GetGroupItems(
                        g.Key,
                        t.Key.GroupType,
                        t.Key.DisplayType,
                        request.UplineRoleId,
                        request.Language,
                        groupItems, t)
                };

                return groupType;
            }

            return null;
        }).ToEnumerable();
    }

    private IEnumerable<GroupItemDefault> GetGroupItems(
        int groupId,
        int groupTypeId,
        int displayType,
        int uplineRoleId,
        string language,
        IEnumerable<GroupItemDefault> groupItems, 
        IGrouping<object, SettingProfile> t)
    {
        return groupItems.Join(
        t.GroupBy(sb => new { sb.SubGroupId, sb.SportType, sb.BetType }),
        groupItems => new { SportType = groupItems.SportTypeId, BetType = groupItems.BetTypeId },
        groupItemsProfile => new { groupItemsProfile.Key.SportType, groupItemsProfile.Key.BetType },
        (groupItem, groupItemsProfile) =>
        {
            groupItem.SubGroupId = groupItemsProfile.Key.SubGroupId;
            groupItem.SubGroupName = cacheRefService.GetSubGroupName(groupItemsProfile.Key.SubGroupId, language);
            groupItem.SportTypeName = cacheRefService.GetSportTypeName(groupItem.SportTypeId, language);
            groupItem.BetTypeName = cacheRefService.GetBetTypeName(groupItem.BetTypeId, language);
            groupItem.DisplayName = GetDisplayNameByDisplayType(
            groupItem.SportTypeId,
            groupItem.BetTypeId,
            groupId,
            groupTypeId,
            displayType,
            uplineRoleId,
            language);

            return groupItem;
        });
    }
}