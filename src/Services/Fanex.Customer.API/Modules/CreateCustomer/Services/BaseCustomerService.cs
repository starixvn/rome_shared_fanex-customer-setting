using Fanex.Customer.API.Modules.CreateCustomer.Helpers;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType158;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType160;
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
            {150_4, new GroupTypeBaseService<SuperCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {150_3, new GroupTypeBaseService<MasterCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {150_2, new GroupTypeBaseService<AgentCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {150_1, new GroupTypeBaseService<MemberCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {151, new GroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {152, new GroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_4, new GroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_3, new GroupTypeBaseService<MasterPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_2, new GroupTypeBaseService<AgentPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1, new GroupTypeBaseService<AgentPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_4, new GroupTypeBaseService<SuperPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_3, new GroupTypeBaseService<MasterPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {154, new GroupTypeBaseService<ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {155, new GroupTypeBaseService<WinLossLimitItem>(dynamicRepository, appSettings, helperService)},
            {156_4, new GroupTypeBaseService<SuperCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {156_3, new GroupTypeBaseService<MasterCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {156_2, new GroupTypeBaseService<AgentCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {156_1, new GroupTypeBaseService<MemberCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {157, new GroupTypeBaseService<ViewModels.GroupType157.BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {158, new GroupTypeBaseService<StakeLevelItem>(dynamicRepository, appSettings, helperService)},
            {160, new GroupTypeBaseService<ProductStatusItem>(dynamicRepository, appSettings, helperService)},
        };

    }

    protected static bool IsGroupTypeByLevel(int groupType)
    => groupType == 153 || groupType == 150 || groupType == 156;

    protected bool IsSabaCasino(int sportTypeId, int betTypeId)
    => sportTypeId == 208 && betTypeId == 2101;

    protected bool IsPTGroupType(int groupType)
    => groupType == 153;
}
