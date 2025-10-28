using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public class MockAfterQueryTask : IAfterQueryTask
{
    public int GroupTypeId { get; set; }
    public bool WasExecuted { get; set; }
    public Exception? ThrowException { get; set; }

    public Task ExecuteAsync(CustomerSettingModel customerSetting, AfterQueryContext context)
    {
        if (ThrowException != null)
        {
            throw ThrowException;
        }
        
        WasExecuted = true;
        return Task.CompletedTask;
    }
}

public class AfterQueryServiceTests
{
    private readonly Mock<ILogger<AfterQueryService>> mockLogger;
    private readonly AfterQueryService service;
    private readonly List<MockAfterQueryTask> mockTasks;

    public AfterQueryServiceTests()
    {
        mockLogger = new Mock<ILogger<AfterQueryService>>();
        mockTasks = new List<MockAfterQueryTask>();
        service = new AfterQueryService(mockTasks, mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAllTasksAsync_NullCustomerSetting_ShouldReturn()
    {
        await service.ExecuteAllTasksAsync(null, CreateContext());

        Assert.All(mockTasks, task => Assert.False(task.WasExecuted));
    }

    [Fact]
    public async Task ExecuteAllTasksAsync_NullProductGroups_ShouldReturn()
    {
        var customerSetting = new CustomerSettingModel { ProductGroups = null };

        await service.ExecuteAllTasksAsync(customerSetting, CreateContext());

        Assert.All(mockTasks, task => Assert.False(task.WasExecuted));
    }

    [Fact]
    public async Task ExecuteAllTasksAsync_NoMatchingGroupTypes_ShouldNotExecuteTasks()
    {
        var customerSetting = CreateCustomerSettingWithGroupTypes(999, 888);
        var task1 = new MockAfterQueryTask { GroupTypeId = 156 };
        var task2 = new MockAfterQueryTask { GroupTypeId = 153 };
        mockTasks.AddRange(new[] { task1, task2 });

        await service.ExecuteAllTasksAsync(customerSetting, CreateContext());

        Assert.False(task1.WasExecuted);
        Assert.False(task2.WasExecuted);
    }

    [Fact]
    public async Task ExecuteAllTasksAsync_WithMatchingGroupTypes_ShouldExecuteApplicableTasks()
    {
        var customerSetting = CreateCustomerSettingWithGroupTypes(156, 160);
        var task1 = new MockAfterQueryTask { GroupTypeId = 156 }; // Should execute
        var task2 = new MockAfterQueryTask { GroupTypeId = 153 }; // Should not execute
        var task3 = new MockAfterQueryTask { GroupTypeId = 160 }; // Should execute
        mockTasks.AddRange(new[] { task1, task2, task3 });

        await service.ExecuteAllTasksAsync(customerSetting, CreateContext());

        Assert.True(task1.WasExecuted);
        Assert.False(task2.WasExecuted);
        Assert.True(task3.WasExecuted);
    }

    [Fact]
    public async Task ExecuteAllTasksAsync_TaskThrowsException_ShouldLogErrorAndContinue()
    {
        var customerSetting = CreateCustomerSettingWithGroupTypes(156, 153);
        var task1 = new MockAfterQueryTask 
        { 
            GroupTypeId = 156, 
            ThrowException = new InvalidOperationException("Test error") 
        };
        var task2 = new MockAfterQueryTask { GroupTypeId = 153 };
        mockTasks.AddRange(new[] { task1, task2 });

        await service.ExecuteAllTasksAsync(customerSetting, CreateContext());

        // task1 should not set WasExecuted due to exception
        Assert.False(task1.WasExecuted);
        // task2 should still execute despite task1 throwing
        Assert.True(task2.WasExecuted);
        
        // Verify error was logged
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error executing after query task for GroupType 156")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAllTasksAsync_DuplicateGroupTypes_ShouldExecuteCorrectly()
    {
        var customerSetting = new CustomerSettingModel
        {
            ProductGroups = new List<ProductGroup>
            {
                new ProductGroup
                {
                    GroupTypes = new List<GroupType>
                    {
                        new GroupType { Id = 156 },
                        new GroupType { Id = 156 } // Duplicate
                    }
                }
            }
        };

        var task = new MockAfterQueryTask { GroupTypeId = 156 };
        mockTasks.Add(task);

        await service.ExecuteAllTasksAsync(customerSetting, CreateContext());

        Assert.True(task.WasExecuted);
    }

    [Fact]
    public async Task ExecuteAllTasksAsync_EmptyGroupTypes_ShouldNotExecuteTasks()
    {
        var customerSetting = new CustomerSettingModel
        {
            ProductGroups = new List<ProductGroup>
            {
                new ProductGroup { GroupTypes = new List<GroupType>() }
            }
        };

        var task = new MockAfterQueryTask { GroupTypeId = 156 };
        mockTasks.Add(task);

        await service.ExecuteAllTasksAsync(customerSetting, CreateContext());

        Assert.False(task.WasExecuted);
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

    private CustomerSettingModel CreateCustomerSettingWithGroupTypes(params int[] groupTypeIds)
    {
        return new CustomerSettingModel
        {
            ProductGroups = new List<ProductGroup>
            {
                new ProductGroup
                {
                    GroupTypes = groupTypeIds.Select(id => new GroupType { Id = id }).ToList()
                }
            }
        };
    }
}