using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.QueryCustomer.Requests;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;

namespace Fanex.Customer.API.Modules.QueryCustomer.Services;

public interface IPTSettingService
{
    Task<IEnumerable<ProductGroup>> GetPTSettings(QueryPTTemplateRequest request);
}

public class PTSettingService : BaseQueryCustomerService, IPTSettingService
{
    public PTSettingService(
        IDynamicRepository dynamicRepository, 
        IOptions<AppSettings> appSettings, 
        ISettingProfileService settingProfileService, 
        ICacheRefService cacheRefService, 
        IMessageService messageService,
        ICustomerSettingHelper helperService)
        : base(dynamicRepository, appSettings, settingProfileService, cacheRefService, messageService, null, helperService)
    {
    }

    public async Task<IEnumerable<ProductGroup>> GetPTSettings(QueryPTTemplateRequest request)
    {
        var settingProfile = await settingProfileService.GetSettingProfiles(request.SystemId, request.Site, request.Currency, SettingProfileService.ProfileMode.AddNew);
        var productGroups = settingProfile.Where(p => p.GroupType == 153).GroupBy(p => p.GroupId)
            .Select((g) => new ProductGroup
            {
                Id = g.Key,
                Name = cacheRefService.GetProductGroupName(g.Key, request.Language),
                GroupTypes = g.GroupBy(t => new { t.GroupType, t.DisplayType }).Select(t => t).ToAsyncEnumerable().SelectAwait(async t =>
                {
                    var groupTypeWithRoleId = helperService.IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevel(t.Key.GroupType, (byte)(request.RoleId - 1)) : t.Key.GroupType;
                    var groupItems = await groupItemMapping[groupTypeWithRoleId].QueryForAddNew(request.CustId, request.RoleId, g.Key, t.Key.GroupType, false);
                    var groupType = new GroupType
                    {
                        Id = t.Key.GroupType,
                        Name = Resources.Resources.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
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

                                return groupItem;
                            })
                    };

                    return groupType;
                }).ToEnumerable().ToList() // Must use ToList to prevent call SP 2 times because of Any method in the next action.
            }).Where(p => p.GroupTypes.Any(i => i.GroupItems.Any()));

        return productGroups;
    }
}
