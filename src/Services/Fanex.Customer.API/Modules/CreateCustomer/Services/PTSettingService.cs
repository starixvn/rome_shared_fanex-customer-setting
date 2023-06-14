using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Resources;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface IPTSettingService
{
    Task<IEnumerable<ProductGroup>> GetPTSettings(int custId, string language);
}

public class PTSettingService: BaseCustomerService, IPTSettingService
{
    public PTSettingService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ISharedService sharedService, ICacheRefService cacheRefService, IMessageService messageService)
        : base(dynamicRepository, appSettings, sharedService, cacheRefService, messageService)
    {
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

        var productGroups = settingProfile.Where(p => p.GroupType == 153 || p.GroupType == 160).GroupBy(p => p.GroupId)
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
}
