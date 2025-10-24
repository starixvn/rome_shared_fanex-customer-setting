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
using System.Text.RegularExpressions;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Entities;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Criteria;

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

    protected async Task SetProductStatusTooltip(CustomerSettingModel customerSetting, string language, int custId, byte settingStatus)
    {
        const int PRODUCT_STATUS_GROUP_TYPE = 160;
        const int JACKPOT_SPORT_TYPE_ID = 9904;
        const int JACKPOT_BET_TYPE_ID = 4001;

        var result = await dynamicRepository.FetchAsync<JackpotStatusTooltipProduct>(new GetJackpotStatusTooltipCriteria(custId, settingStatus));
        if (result == null || !result.Any())
        {
            return;
        }

        var productNames = result
            .Select(item => cacheRefService.GetProductName(item.RealProductId, language))
            .Where(name => !string.IsNullOrEmpty(name));

        if (!productNames.Any())
        {
            return;
        }

        string tooltipTemplate = Resources.Resources.ResourceManager.GetString("JackpotStatusTooltip", new System.Globalization.CultureInfo(language));
        string tooltipMessage = string.Format(tooltipTemplate, string.Join(", ", productNames));

        // Set tooltip for all group types with PRODUCT_STATUS_GROUP_TYPE
        var productGroupsList = customerSetting.ProductGroups?.ToList();
        if (productGroupsList != null)
        {
            foreach (var productGroup in productGroupsList)
            {
                if (productGroup.GroupTypes != null)
                {
                    var groupTypesList = productGroup.GroupTypes.ToList();
                    foreach (var groupType in groupTypesList)
                    {
                        if (groupType.Id == PRODUCT_STATUS_GROUP_TYPE && groupType.GroupItems != null)
                        {
                            var groupItemsList = groupType.GroupItems.ToList();
                            foreach (var groupItem in groupItemsList)
                            {
                                if (groupItem.SportTypeId == JACKPOT_SPORT_TYPE_ID && groupItem.BetTypeId == JACKPOT_BET_TYPE_ID)
                                {
                                    groupItem.TooltipMessage = tooltipMessage;
                                }
                            }
                            groupType.GroupItems = groupItemsList;
                        }
                    }
                    productGroup.GroupTypes = groupTypesList;
                }
            }
            customerSetting.ProductGroups = productGroupsList;
        }
    }

#pragma warning disable S1067, S138, S3776, S3353, S134, S126 // Complexity and method length warnings
    protected async Task SetCommAndPTTooltip(CustomerSettingModel customerSetting, string language, int custId, byte settingStatus)
    {
        const int COMMISSION_GROUP_TYPE = 156;
        const int PT_GROUP_TYPE = 153;
        const int SPORT_TYPE_ID_9916 = 9916;
        const int SPORT_TYPE_ID_9917 = 9917;
        const int BET_TYPE_ID = 2400;

        // Hardcoded tooltip messages
        string tooltip9916 = "Product: Allbet, SA Gaming, PP Live, ION, WE";
        string tooltip9917 = "Product: AE Sexy, ON Casino, Playtech Live, MG Live, WM, BG, Yeebet, W Casino, GPI Live";

        // Set tooltip for all group types with COMMISSION_GROUP_TYPE and PT_GROUP_TYPE
        var productGroupsList = customerSetting.ProductGroups?.ToList();
        if (productGroupsList != null)
        {
            foreach (var productGroup in productGroupsList)
            {
                if (productGroup.GroupTypes != null)
                {
                    var groupTypesList = productGroup.GroupTypes.ToList();
                    foreach (var groupType in groupTypesList)
                    {
                        if ((groupType.Id == COMMISSION_GROUP_TYPE || groupType.Id == PT_GROUP_TYPE) && groupType.GroupItems != null)
                        {
                            var groupItemsList = groupType.GroupItems.ToList();
                            foreach (var groupItem in groupItemsList)
                            {
                                if (groupItem.BetTypeId == BET_TYPE_ID)
                                {
                                    if (groupItem.SportTypeId == SPORT_TYPE_ID_9916)
                                    {
                                        groupItem.TooltipMessage = tooltip9916;
                                    }
                                    else if (groupItem.SportTypeId == SPORT_TYPE_ID_9917)
                                    {
                                        groupItem.TooltipMessage = tooltip9917;
                                    }
                                }
                            }
                            groupType.GroupItems = groupItemsList;
                        }
                    }
                    productGroup.GroupTypes = groupTypesList;
                }
            }
            customerSetting.ProductGroups = productGroupsList;
        }
    }
#pragma warning restore S1067, S138, S3776, S3353, S134, S126
}
