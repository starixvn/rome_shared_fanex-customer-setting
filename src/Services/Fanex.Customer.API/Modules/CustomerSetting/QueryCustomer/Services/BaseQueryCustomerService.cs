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
            {150_4, new QueryGroupTypeBaseService<SuperCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {150_3, new QueryGroupTypeBaseService<MasterCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {150_2, new QueryGroupTypeBaseService<AgentCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {150_1, new QueryGroupTypeBaseService<MemberCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {151, new QueryGroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {152, new QueryGroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_4, new QueryGroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_3, new QueryGroupTypeBaseService<MasterPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_2, new QueryGroupTypeBaseService<AgentPTSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1, new QueryGroupTypeBaseService<AgentPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_4, new QueryGroupTypeBaseService<SuperPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {153_1_3, new QueryGroupTypeBaseService<MasterPTOnMemberSettingItem>(dynamicRepository, appSettings, helperService)},
            {154, new QueryGroupTypeBaseService<CustomerSetting.Shared.ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {155, new QueryGroupTypeBaseService<WinLossLimitItem>(dynamicRepository, appSettings, helperService)},
            {156_4, new QueryGroupTypeBaseService<SuperCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {156_3, new QueryGroupTypeBaseService<MasterCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {156_2, new QueryGroupTypeBaseService<AgentCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {156_1, new QueryGroupTypeBaseService<MemberCommissionGroup>(dynamicRepository, appSettings, helperService)},
            {157, new QueryGroupTypeBaseService<CustomerSetting.Shared.ViewModels.GroupType157.BetSettingItem>(dynamicRepository, appSettings, helperService)},
            {158, new QueryGroupTypeBaseService<StakeLevelItem>(dynamicRepository, appSettings, helperService)},
            {160, new QueryGroupTypeBaseService<ProductStatusItem>(dynamicRepository, appSettings, helperService)},
        };

    }
}
