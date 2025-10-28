using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Entities;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Criteria;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Moq;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class ProductStatusTooltipTaskTests
{
    private readonly Mock<IDynamicRepository> mockDynamicRepository;
    private readonly Mock<ICacheRefService> mockCacheRefService;
    private readonly ProductStatusTooltipTask task;

    public ProductStatusTooltipTaskTests()
    {
        mockDynamicRepository = new Mock<IDynamicRepository>();
        mockCacheRefService = new Mock<ICacheRefService>();
        task = new ProductStatusTooltipTask(mockDynamicRepository.Object, mockCacheRefService.Object);
    }

    [Fact]
    public void GroupTypeId_ShouldReturn160()
    {
        Assert.Equal(160, task.GroupTypeId);
    }

    [Fact]
    public async Task ExecuteAsync_NoData_ShouldNotSetTooltip()
    {
        var context = CreateContext();
        var customerSetting = CreateCustomerSetting();

        mockDynamicRepository.Setup(x => x.FetchAsync<JackpotStatusTooltipProduct>(It.IsAny<GetJackpotStatusTooltipCriteria>()))
            .ReturnsAsync(new List<JackpotStatusTooltipProduct>());

        await task.ExecuteAsync(customerSetting, context);

        var groupItem = GetJackpotGroupItem(customerSetting);
        Assert.Null(groupItem.TooltipMessage);
    }

    [Fact]
    public async Task ExecuteAsync_NullResult_ShouldNotSetTooltip()
    {
        var context = CreateContext();
        var customerSetting = CreateCustomerSetting();

        mockDynamicRepository.Setup(x => x.FetchAsync<JackpotStatusTooltipProduct>(It.IsAny<GetJackpotStatusTooltipCriteria>()))
            .ReturnsAsync((IEnumerable<JackpotStatusTooltipProduct>?)null);

        await task.ExecuteAsync(customerSetting, context);

        var groupItem = GetJackpotGroupItem(customerSetting);
        Assert.Null(groupItem.TooltipMessage);
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
                            Id = 160,
                            GroupItems = new List<GroupItemDefault>
                            {
                                new GroupItemDefault 
                                { 
                                    SportTypeId = 9904, 
                                    BetTypeId = 4001,
                                    TooltipMessage = null
                                },
                                new GroupItemDefault 
                                { 
                                    SportTypeId = 1, 
                                    BetTypeId = 1,
                                    TooltipMessage = null
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private GroupItemDefault GetJackpotGroupItem(CustomerSettingModel customerSetting)
    {
        return customerSetting.ProductGroups.First()
            .GroupTypes.First(gt => gt.Id == 160)
            .GroupItems.First(gi => gi.SportTypeId == 9904 && gi.BetTypeId == 4001);
    }
}