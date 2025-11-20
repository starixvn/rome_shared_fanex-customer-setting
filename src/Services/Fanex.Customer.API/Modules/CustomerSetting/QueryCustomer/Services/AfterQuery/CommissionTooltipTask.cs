using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class CommissionTooltipTask : LiveCasinoGroupTooltipTask
{
    public override int GroupTypeId => GroupTypes.Commission;

    protected override string TooltipResourceKey => "MessageTooltip_LiveCasinoGroup_Commission";

    public CommissionTooltipTask(
        IDynamicRepository dynamicRepository, 
        ICacheRefService cacheRefService,
        IMemoryCache memoryCache) 
        : base(dynamicRepository, cacheRefService, memoryCache)
    {
    }
}