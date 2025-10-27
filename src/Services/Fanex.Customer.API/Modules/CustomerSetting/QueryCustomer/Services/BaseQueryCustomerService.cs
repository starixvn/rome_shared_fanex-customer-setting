using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType158;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType160;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.EditCustomer;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType161;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType162;

namespace Fanex.Customer.API.Modules.QueryCustomer.Services;

public class BaseQueryCustomerService : BaseCustomerService
{
    protected readonly Dictionary<int, dynamic> groupItemMapping;

    protected BaseQueryCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISettingProfileService settingProfileService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        IUMService uMService,
        ICustomerSettingHelper helperService)
        : base(dynamicRepository, appSettings, settingProfileService, cacheRefService, messageService, uMService, helperService)
    {
        groupItemMapping = new()
        {
            {150, new QueryGroupTypeBaseService<CommissionGroup>(dynamicRepository, appSettings, helperService)},
            {151, new QueryGroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {152, new QueryGroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_4, new QueryGroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_3, new QueryGroupTypeBaseService<MasterPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_2, new QueryGroupTypeBaseService<AgentPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1, new QueryGroupTypeBaseService<AgentPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_4, new QueryGroupTypeBaseService<SuperPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_4_1, new QueryGroupTypeBaseService<SuperPTOnDirectMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_3, new QueryGroupTypeBaseService<MasterPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_3_1, new QueryGroupTypeBaseService<MasterPTOnDirectMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_4_1_3, new QueryGroupTypeBaseService<SuperPTOnDirectMasterMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {154, new QueryGroupTypeBaseService<CustomerSetting.Shared.ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {155, new QueryGroupTypeBaseService<WinLossLimitItem>(dynamicRepository, appSettings, helperService)},
            {156, new QueryGroupTypeBaseService<CommissionGroup>(dynamicRepository, appSettings, helperService)},
            {157, new QueryGroupTypeBaseService<CustomerSetting.Shared.ViewModels.GroupType157.BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {158, new QueryGroupTypeBaseService<StakeLevelItem>(dynamicRepository, appSettings, helperService)},
            {160, new QueryGroupTypeBaseService<ProductStatusItem>(dynamicRepository, appSettings, helperService)},
            {161, new QueryGroupTypeBaseService<WinLimitItem>(dynamicRepository, appSettings, helperService)},
            {GroupTypes.PotentialWinLimit, new QueryGroupType162BaseService<PotentialWinLimit>(dynamicRepository, appSettings, helperService)},
        };
    }

    protected bool IsHideGroupType(int displayMode)
        => displayMode == DisplayMode.Hidden;
}
