using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Criteria;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Entities;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public abstract class LiveCasinoGroupTooltipTask : BaseTooltipTask
{
    private const int CACHE_DURATION_HOURS = 1;
    
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheRefService cacheRefService;
    private readonly IMemoryCache memoryCache;

    protected LiveCasinoGroupTooltipTask(
        IDynamicRepository dynamicRepository, 
        ICacheRefService cacheRefService,
        IMemoryCache memoryCache)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheRefService = cacheRefService;
        this.memoryCache = memoryCache;
    }

    public override async Task ExecuteAsync(CustomerSettingModel customerSetting, AfterQueryContext context)
    {
        var cacheKey = $"LiveCasinoTooltip_{context.Site}_{context.Currency}_{context.SettingStatus}";
        var tooltipProducts = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CACHE_DURATION_HOURS);
            return await dynamicRepository.FetchAsync<LiveCasinoTooltipProduct>(
                new GetLiveCasinoTooltipCriteria(context.Site, context.Currency, context.SettingStatus));
        });

        if (tooltipProducts == null || !tooltipProducts.Any())
        {
            return;
        }

        var tooltipMappings = GenerateTooltipMappings(tooltipProducts, context);
        
        if (tooltipMappings.Count != 0)
        {
            SetTooltipForGroupItems(customerSetting, GroupTypeId, tooltipMappings);
        }
    }

    internal Dictionary<(int SportTypeId, int BetTypeId), string> GenerateTooltipMappings(
        IEnumerable<LiveCasinoTooltipProduct> tooltipProducts, 
        AfterQueryContext context)
    {
        var tooltipMappings = new Dictionary<(int SportTypeId, int BetTypeId), string>();

        var relevantProducts = tooltipProducts
            .Where(p => p.GroupType == GroupTypeId)
            .GroupBy(p => new { p.SportType, p.BetType });

        foreach (var productGroup in relevantProducts)
        {
            var key = (productGroup.Key.SportType, productGroup.Key.BetType);
            
            var productNames = productGroup
                .Select(p => cacheRefService.GetProductName(p.RealProductId, context.Language))
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct();

            if (productNames.Any())
            {
                string tooltipTemplate = Resources.Resources.ResourceManager.GetString(
                    "MessageTooltip_LiveCasinoGroup", 
                    new System.Globalization.CultureInfo(context.Language));
                var tooltipMessage = string.Format(tooltipTemplate, string.Join(", ", productNames));
                tooltipMappings[key] = tooltipMessage;
            }
        }

        return tooltipMappings;
    }
}