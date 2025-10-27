using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public interface IAfterQueryService
{
    Task ExecuteAllTasksAsync(CustomerSettingModel customerSetting, AfterQueryContext context);
}

public class AfterQueryService : IAfterQueryService
{
    private readonly IEnumerable<IAfterQueryTask> afterQueryTasks;
    private readonly ILogger<AfterQueryService> logger;

    public AfterQueryService(IEnumerable<IAfterQueryTask> afterQueryTasks, ILogger<AfterQueryService> logger)
    {
        this.afterQueryTasks = afterQueryTasks;
        this.logger = logger;
    }

    public async Task ExecuteAllTasksAsync(CustomerSettingModel customerSetting, AfterQueryContext context)
    {
        if (customerSetting?.ProductGroups == null)
        {
            return;
        }

        var existingGroupTypeIds = customerSetting.ProductGroups
            .Where(pg => pg.GroupTypes != null)
            .SelectMany(pg => pg.GroupTypes)
            .Select(gt => gt.Id)
            .Distinct()
            .ToHashSet();

        var applicableTasks = afterQueryTasks.Where(task => existingGroupTypeIds.Contains(task.GroupTypeId));

        foreach (var task in applicableTasks)
        {
            try
            {
                await task.ExecuteAsync(customerSetting, context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing after query task for GroupType {GroupTypeId}", task.GroupTypeId);
            }
        }
    }
}