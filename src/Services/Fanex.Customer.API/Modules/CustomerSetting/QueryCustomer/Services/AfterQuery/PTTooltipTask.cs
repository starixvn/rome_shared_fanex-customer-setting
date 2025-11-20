using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class PTTooltipTask : LiveCasinoGroupTooltipTask
{
    public override int GroupTypeId => GroupTypes.PositionTaking;

    protected override string TooltipResourceKey => "MessageTooltip_LiveCasinoGroup_PositionTaking";

    public PTTooltipTask(
        IDynamicRepository dynamicRepository, 
        ICacheRefService cacheRefService,
        IMemoryCache memoryCache) 
        : base(dynamicRepository, cacheRefService, memoryCache)
    {
    }
}