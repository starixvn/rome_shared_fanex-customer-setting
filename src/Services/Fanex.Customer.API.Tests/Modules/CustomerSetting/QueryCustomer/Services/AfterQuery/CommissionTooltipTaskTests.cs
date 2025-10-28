using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class CommissionTooltipTaskTests
{
    private readonly Mock<IDynamicRepository> mockDynamicRepository;
    private readonly Mock<ICacheRefService> mockCacheRefService;
    private readonly Mock<IMemoryCache> mockMemoryCache;
    private readonly CommissionTooltipTask task;

    public CommissionTooltipTaskTests()
    {
        mockDynamicRepository = new Mock<IDynamicRepository>();
        mockCacheRefService = new Mock<ICacheRefService>();
        mockMemoryCache = new Mock<IMemoryCache>();
        task = new CommissionTooltipTask(
            mockDynamicRepository.Object,
            mockCacheRefService.Object,
            mockMemoryCache.Object);
    }

    [Fact]
    public void GroupTypeId_ShouldReturn156()
    {
        Assert.Equal(156, task.GroupTypeId);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        Assert.NotNull(task);
        Assert.Equal(156, task.GroupTypeId);
    }

    [Fact]
    public void Task_ShouldInheritFromLiveCasinoGroupTooltipTask()
    {
        Assert.IsAssignableFrom<LiveCasinoGroupTooltipTask>(task);
    }
}