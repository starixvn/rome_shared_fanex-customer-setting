using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Criteria;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Entities;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class ProductStatusTooltipTask : BaseTooltipTask
{
    private const int JACKPOT_SPORT_TYPE_ID = 9904;
    private const int JACKPOT_BET_TYPE_ID = 4001;

    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheRefService cacheRefService;

    public override int GroupTypeId => GroupTypes.ProductStatus;

    public ProductStatusTooltipTask(IDynamicRepository dynamicRepository, ICacheRefService cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheRefService = cacheRefService;
    }

    public override async Task ExecuteAsync(CustomerSettingModel customerSetting, AfterQueryContext context)
    {
        var result = await dynamicRepository.FetchAsync<JackpotStatusTooltipProduct>(
            new GetJackpotStatusTooltipCriteria(context.CustId, context.SettingStatus));
        
        if (result == null || !result.Any())
        {
            return;
        }

        var productNames = result
            .Select(item => cacheRefService.GetProductName(item.RealProductId, context.Language))
            .Where(name => !string.IsNullOrEmpty(name));

        if (!productNames.Any())
        {
            return;
        }

        string tooltipTemplate = Resources.Resources.ResourceManager.GetString(
            "JackpotStatusTooltip", 
            new System.Globalization.CultureInfo(context.Language));
        string tooltipMessage = string.Format(tooltipTemplate, string.Join(", ", productNames));

        SetTooltipForGroupItems(
            customerSetting, 
            GroupTypes.ProductStatus,
            item => item.SportTypeId == JACKPOT_SPORT_TYPE_ID && item.BetTypeId == JACKPOT_BET_TYPE_ID,
            tooltipMessage);
    }
}