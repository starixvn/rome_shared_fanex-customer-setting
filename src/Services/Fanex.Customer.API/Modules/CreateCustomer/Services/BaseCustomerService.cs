using Fanex.Customer.API.Modules.CreateCustomer.Helpers;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.EditCustomer;
using Fanex.Customer.API.Modules.Message;
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
    protected readonly IUMService uMService;
    protected readonly ICustomerSettingHelper helperService;
    protected readonly Dictionary<int, dynamic> groupItemMapping;

    protected const int SabaCasinoProductId = 6;

    protected BaseCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISharedService sharedService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        IUMService uMService,
        ICustomerSettingHelper helperService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.sharedService = sharedService;
        this.cacheRefService = cacheRefService;
        this.messageService = messageService;
        this.uMService = uMService;
        this.helperService = helperService;

        groupItemMapping = new()
        {
            {150_4, new GroupTypeBaseService<SuperCommissionGroup>(dynamicRepository, appSettings.Value, helperService)},
            {150_3, new GroupTypeBaseService<MasterCommissionGroup>(dynamicRepository, appSettings.Value, helperService)},
            {150_2, new GroupTypeBaseService<AgentCommissionGroup>(dynamicRepository, appSettings.Value, helperService)},
            {150_1, new GroupTypeBaseService<MemberCommissionGroup>(dynamicRepository, appSettings.Value, helperService)},
            {151, new GroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {152, new GroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {153_4, new GroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {153_3, new GroupTypeBaseService<MasterPTSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {153_2, new GroupTypeBaseService<AgentPTSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {153_1, new GroupTypeBaseService<AgentPTOnMemberSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {153_1_4, new GroupTypeBaseService<SuperPTOnMemberSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {153_1_3, new GroupTypeBaseService<MasterPTOnMemberSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {154, new GroupTypeBaseService<ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {156, new GroupTypeBaseService<CommissionSettingItem>(dynamicRepository, appSettings.Value, helperService)},
            {157, new GroupTypeBaseService<ViewModels.GroupType157.BetSettingItem>(dynamicRepository, appSettings.Value, helperService)},
        };

    }

    // protected string GetDisplayNameByDisplayType(
    //     int sportTypeId,
    //     int betTypeId,
    //     int productGroupId,
    //     int groupTypeId,
    //     int displayType,
    //     int roleId,
    //     string language,
    //     bool isSiteLabel777)
    //     if (displayType == (int)DisplayType.CommSB)
    //     {
    //         if (roleId == (int)CustomerLevel.Member)
    //         {
    //             return cacheRefService.GetSportsbookCommMemberName(
    //                 productGroupId,
    //                 groupTypeId,
    //                 sportTypeId,
    //                 betTypeId,
    //                 language);
    //         }
    //         else
    //         {
    //             return cacheRefService.GetSportsbookCommSMAName(
    //                 productGroupId,
    //                 groupTypeId,
    //                 sportTypeId,
    //                 betTypeId,
    //                 language);
    //         }
    //     }
    //     else
    //     {
    //         return cacheRefService.GetNameByGroupSubGroup(
    //                 productGroupId,
    //                 groupTypeId,
    //                 sportTypeId,
    //                 betTypeId,
    //                 language);
    //     }
    // }

    protected static bool IsGroupTypeByLevel(int groupType)
    => groupType == 153 || groupType == 150;

    protected bool IsSabaCasino(int sportTypeId, int betTypeId)
    => sportTypeId == 208 && betTypeId == 2101;

    protected bool IsPTGroupType(int groupType)
    => groupType == 153;
}
