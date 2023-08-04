using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public class BaseCustomerService
{
    protected readonly IDynamicRepository dynamicRepository;
    protected readonly AppSettings appSettings;
    protected readonly ISharedService sharedService;
    protected readonly ICacheRefService cacheRefService;
    protected readonly IMessageService messageService;
    protected readonly Dictionary<int, dynamic> groupItemMapping;

    protected BaseCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISharedService sharedService,
        ICacheRefService cacheRefService,
        IMessageService messageService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.sharedService = sharedService;
        this.cacheRefService = cacheRefService;
        this.messageService = messageService;

        groupItemMapping = new()
        {
            {150, new GroupTypeBaseService<CommissionGroup>(dynamicRepository, appSettings.Value)},
            {151, new GroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings.Value)},
            {152, new GroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings.Value)},
            {153_3, new GroupTypeBaseService<MasterPTSettingItem>(dynamicRepository, appSettings.Value)},
            {153_2, new GroupTypeBaseService<AgentPTSettingItem>(dynamicRepository, appSettings.Value)},
            {153_1, new GroupTypeBaseService<MemberPTItem>(dynamicRepository, appSettings.Value)},
            {154, new GroupTypeBaseService<ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings.Value)},
            {156, new GroupTypeBaseService<CommissionSettingItem>(dynamicRepository, appSettings.Value)},
            {157, new GroupTypeBaseService<ViewModels.GroupType157.BetSettingItem>(dynamicRepository, appSettings.Value)},
        };
    }

    protected string GetDisplayNameByDisplayType(
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
                return cacheRefService.GetSportsbookCommMemberName(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
            }
            else
            {
                return cacheRefService.GetSportsbookCommSMAName(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
            }
        }
        else
        {
            return cacheRefService.GetNameByGroupSubGroup(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
        }
    }

    protected static int GetPTGroupTypeByCustomerLevel(int groupType, int roleId)
    => groupType * 10 + roleId;

    protected static bool IsPTGroupType(int groupType)
    => groupType == 153;
}
