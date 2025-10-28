using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Entities;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Criteria;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class TestableLiveCasinoGroupTooltipTask : LiveCasinoGroupTooltipTask
{
    public override int GroupTypeId => 156;

    public TestableLiveCasinoGroupTooltipTask(
        IDynamicRepository dynamicRepository,
        ICacheRefService cacheRefService,
        IMemoryCache memoryCache)
        : base(dynamicRepository, cacheRefService, memoryCache)
    {
    }

    public Dictionary<(int SportTypeId, int BetTypeId), string> TestGenerateTooltipMappings(
        IEnumerable<LiveCasinoTooltipProduct> tooltipProducts,
        AfterQueryContext context)
    {
        return GenerateTooltipMappings(tooltipProducts, context);
    }
}

public class LiveCasinoGroupTooltipTaskTests
{
    private readonly Mock<IDynamicRepository> mockDynamicRepository;
    private readonly Mock<ICacheRefService> mockCacheRefService;
    private readonly Mock<IMemoryCache> mockMemoryCache;
    private readonly TestableLiveCasinoGroupTooltipTask task;

    public LiveCasinoGroupTooltipTaskTests()
    {
        mockDynamicRepository = new Mock<IDynamicRepository>();
        mockCacheRefService = new Mock<ICacheRefService>();
        mockMemoryCache = new Mock<IMemoryCache>();
        task = new TestableLiveCasinoGroupTooltipTask(
            mockDynamicRepository.Object,
            mockCacheRefService.Object,
            mockMemoryCache.Object);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyData_ShouldNotSetTooltip()
    {
        var context = CreateContext();
        var customerSetting = CreateCustomerSetting();

        SetupMemoryCache(new List<LiveCasinoTooltipProduct>());

        await task.ExecuteAsync(customerSetting, context);

        var groupItem = GetGroupItem(customerSetting, 1, 2);
        Assert.Null(groupItem.TooltipMessage);
    }

    [Fact]
    public void GenerateTooltipMappings_EmptyProductNames_ShouldReturnEmptyMappings()
    {
        var context = CreateContext();
        var tooltipProducts = CreateTooltipProducts();

        var result = task.TestGenerateTooltipMappings(tooltipProducts, context);

        Assert.Empty(result);
    }

    [Fact]
    public void GenerateTooltipMappings_DifferentGroupType_ShouldFilterCorrectly()
    {
        var context = CreateContext();
        var tooltipProducts = new List<LiveCasinoTooltipProduct>
        {
            new LiveCasinoTooltipProduct { GroupType = 156, SportType = 1, BetType = 2, RealProductId = 1 },
            new LiveCasinoTooltipProduct { GroupType = 999, SportType = 3, BetType = 4, RealProductId = 2 }
        };

        var result = task.TestGenerateTooltipMappings(tooltipProducts, context);

        Assert.Empty(result); // No product names setup, should be empty
    }

    private void SetupMemoryCache(IEnumerable<LiveCasinoTooltipProduct>? data)
    {
        var mockCacheEntry = new Mock<ICacheEntry>();
        mockCacheEntry.SetupAllProperties();

        mockMemoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);

        if (data != null)
        {
            mockDynamicRepository.Setup(x => x.FetchAsync<LiveCasinoTooltipProduct>(It.IsAny<GetLiveCasinoTooltipCriteria>()))
                .ReturnsAsync(data);
        }
        else
        {
            mockDynamicRepository.Setup(x => x.FetchAsync<LiveCasinoTooltipProduct>(It.IsAny<GetLiveCasinoTooltipCriteria>()))
                .ReturnsAsync((IEnumerable<LiveCasinoTooltipProduct>?)null);
        }
    }

    private AfterQueryContext CreateContext()
    {
        return new AfterQueryContext
        {
            Language = "en",
            CustId = 123,
            SettingStatus = 1,
            Site = "TestSite",
            Currency = 1
        };
    }

    private CustomerSettingModel CreateCustomerSetting()
    {
        return new CustomerSettingModel
        {
            ProductGroups = new List<ProductGroup>
            {
                new ProductGroup
                {
                    GroupTypes = new List<GroupType>
                    {
                        new GroupType
                        {
                            Id = 156,
                            GroupItems = new List<GroupItemDefault>
                            {
                                new GroupItemDefault { SportTypeId = 1, BetTypeId = 2 },
                                new GroupItemDefault { SportTypeId = 3, BetTypeId = 4 }
                            }
                        }
                    }
                }
            }
        };
    }

    private List<LiveCasinoTooltipProduct> CreateTooltipProducts()
    {
        return new List<LiveCasinoTooltipProduct>
        {
            new LiveCasinoTooltipProduct
            {
                GroupType = 156,
                SportType = 1,
                BetType = 2,
                RealProductId = 1
            },
            new LiveCasinoTooltipProduct
            {
                GroupType = 156,
                SportType = 1,
                BetType = 2,
                RealProductId = 2
            }
        };
    }

    private GroupItemDefault GetGroupItem(CustomerSettingModel customerSetting, int sportTypeId, int betTypeId)
    {
        return customerSetting.ProductGroups.First()
            .GroupTypes.First()
            .GroupItems.First(gi => gi.SportTypeId == sportTypeId && gi.BetTypeId == betTypeId);
    }
}