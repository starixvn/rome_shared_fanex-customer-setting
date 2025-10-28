using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class TestableTooltipTask : BaseTooltipTask
{
    public override int GroupTypeId => 999;

    public override Task ExecuteAsync(CustomerSettingModel customerSetting, AfterQueryContext context)
    {
        return Task.CompletedTask;
    }

    public void TestProcessProductGroupWithFilter(
        ProductGroup productGroup,
        int groupTypeId,
        Func<GroupItemDefault, bool> itemFilter,
        string tooltipMessage)
    {
        ProcessProductGroupWithFilter(productGroup, groupTypeId, itemFilter, tooltipMessage);
    }

    public void TestProcessProductGroupWithMappings(
        ProductGroup productGroup,
        int groupTypeId,
        Dictionary<(int SportTypeId, int BetTypeId), string> tooltipMappings)
    {
        ProcessProductGroupWithMappings(productGroup, groupTypeId, tooltipMappings);
    }

    public void TestProcessGroupTypeWithFilter(
        GroupType groupType,
        int groupTypeId,
        Func<GroupItemDefault, bool> itemFilter,
        string tooltipMessage)
    {
        ProcessGroupTypeWithFilter(groupType, groupTypeId, itemFilter, tooltipMessage);
    }

    public void TestProcessGroupTypeWithMappings(
        GroupType groupType,
        int groupTypeId,
        Dictionary<(int SportTypeId, int BetTypeId), string> tooltipMappings)
    {
        ProcessGroupTypeWithMappings(groupType, groupTypeId, tooltipMappings);
    }

    public void TestSetTooltipForGroupItemsWithFilter(
        CustomerSettingModel customerSetting,
        int groupTypeId,
        Func<GroupItemDefault, bool> itemFilter,
        string tooltipMessage)
    {
        SetTooltipForGroupItems(customerSetting, groupTypeId, itemFilter, tooltipMessage);
    }

    public void TestSetTooltipForGroupItemsWithMappings(
        CustomerSettingModel customerSetting,
        int groupTypeId,
        Dictionary<(int SportTypeId, int BetTypeId), string> tooltipMappings)
    {
        SetTooltipForGroupItems(customerSetting, groupTypeId, tooltipMappings);
    }
}

public class BaseTooltipTaskTests
{
    private readonly TestableTooltipTask task;

    public BaseTooltipTaskTests()
    {
        task = new TestableTooltipTask();
    }

    [Fact]
    public void SetTooltipForGroupItems_WithFilter_ShouldSetTooltip()
    {
        var customerSetting = CreateCustomerSettingWithGroupItems();
        
        task.TestSetTooltipForGroupItemsWithFilter(
            customerSetting,
            999,
            item => item.SportTypeId == 1,
            "Test Tooltip");

        var groupItem = customerSetting.ProductGroups.First()
            .GroupTypes.First()
            .GroupItems.First(i => i.SportTypeId == 1);
        
        Assert.Equal("Test Tooltip", groupItem.TooltipMessage);
    }

    [Fact]
    public void SetTooltipForGroupItems_WithMappings_ShouldSetTooltip()
    {
        var customerSetting = CreateCustomerSettingWithGroupItems();
        var mappings = new Dictionary<(int SportTypeId, int BetTypeId), string>
        {
            { (1, 2), "Tooltip for Sport1-Bet2" },
            { (3, 4), "Tooltip for Sport3-Bet4" }
        };

        task.TestSetTooltipForGroupItemsWithMappings(customerSetting, 999, mappings);

        var groupItems = customerSetting.ProductGroups.First()
            .GroupTypes.First()
            .GroupItems.ToList();

        Assert.Equal("Tooltip for Sport1-Bet2", groupItems[0].TooltipMessage);
        Assert.Equal("Tooltip for Sport3-Bet4", groupItems[1].TooltipMessage);
    }

    [Fact]
    public void ProcessProductGroupWithFilter_NullGroupTypes_ShouldNotThrow()
    {
        var productGroup = new ProductGroup { GroupTypes = null };

        var exception = Record.Exception(() =>
            task.TestProcessProductGroupWithFilter(productGroup, 999, item => true, "Test"));

        Assert.Null(exception);
    }

    [Fact]
    public void ProcessGroupTypeWithFilter_WrongGroupTypeId_ShouldNotSetTooltip()
    {
        var groupType = CreateGroupTypeWithItems(888);

        task.TestProcessGroupTypeWithFilter(groupType, 999, item => true, "Test Tooltip");

        Assert.All(groupType.GroupItems, item => Assert.Null(item.TooltipMessage));
    }

    [Fact]
    public void ProcessGroupTypeWithFilter_CorrectGroupTypeId_ShouldSetTooltip()
    {
        var groupType = CreateGroupTypeWithItems(999);

        task.TestProcessGroupTypeWithFilter(groupType, 999, item => item.SportTypeId == 1, "Test Tooltip");

        var matchingItem = groupType.GroupItems.First(i => i.SportTypeId == 1);
        Assert.Equal("Test Tooltip", matchingItem.TooltipMessage);
    }

    [Fact]
    public void ProcessGroupTypeWithMappings_WithValidMappings_ShouldSetTooltips()
    {
        var groupType = CreateGroupTypeWithItems(999);
        var mappings = new Dictionary<(int SportTypeId, int BetTypeId), string>
        {
            { (1, 2), "Tooltip 1" },
            { (3, 4), "Tooltip 2" }
        };

        task.TestProcessGroupTypeWithMappings(groupType, 999, mappings);

        var items = groupType.GroupItems.ToList();
        Assert.Equal("Tooltip 1", items[0].TooltipMessage);
        Assert.Equal("Tooltip 2", items[1].TooltipMessage);
    }

    [Fact]
    public void ProcessGroupTypeWithMappings_NoMatchingKeys_ShouldNotSetTooltips()
    {
        var groupType = CreateGroupTypeWithItems(999);
        var mappings = new Dictionary<(int SportTypeId, int BetTypeId), string>
        {
            { (99, 88), "Tooltip" }
        };

        task.TestProcessGroupTypeWithMappings(groupType, 999, mappings);

        Assert.All(groupType.GroupItems, item => Assert.Null(item.TooltipMessage));
    }

    private CustomerSettingModel CreateCustomerSettingWithGroupItems()
    {
        return new CustomerSettingModel
        {
            ProductGroups = new List<ProductGroup>
            {
                new ProductGroup
                {
                    GroupTypes = new List<GroupType>
                    {
                        CreateGroupTypeWithItems(999)
                    }
                }
            }
        };
    }

    private GroupType CreateGroupTypeWithItems(int groupTypeId)
    {
        return new GroupType
        {
            Id = groupTypeId,
            GroupItems = new List<GroupItemDefault>
            {
                new GroupItemDefault { SportTypeId = 1, BetTypeId = 2 },
                new GroupItemDefault { SportTypeId = 3, BetTypeId = 4 },
                new GroupItemDefault { SportTypeId = 5, BetTypeId = 6 }
            }
        };
    }
}