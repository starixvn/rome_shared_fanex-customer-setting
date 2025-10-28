using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class PTTooltipTaskTests
{
    private readonly Mock<IDynamicRepository> mockDynamicRepository;
    private readonly Mock<ICacheRefService> mockCacheRefService;
    private readonly Mock<IMemoryCache> mockMemoryCache;
    private readonly PTTooltipTask task;

    public PTTooltipTaskTests()
    {
        mockDynamicRepository = new Mock<IDynamicRepository>();
        mockCacheRefService = new Mock<ICacheRefService>();
        mockMemoryCache = new Mock<IMemoryCache>();
        task = new PTTooltipTask(
            mockDynamicRepository.Object,
            mockCacheRefService.Object,
            mockMemoryCache.Object);
    }

    [Fact]
    public void GroupTypeId_ShouldReturn153()
    {
        Assert.Equal(153, task.GroupTypeId);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        Assert.NotNull(task);
        Assert.Equal(153, task.GroupTypeId);
    }

    [Fact]
    public void Task_ShouldInheritFromLiveCasinoGroupTooltipTask()
    {
        Assert.IsAssignableFrom<LiveCasinoGroupTooltipTask>(task);
    }
}