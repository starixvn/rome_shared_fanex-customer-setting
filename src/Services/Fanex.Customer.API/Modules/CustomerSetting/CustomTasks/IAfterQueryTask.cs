using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

namespace Fanex.Customer.API.Modules.CustomerSetting.CustomTasks.GroupType160;

public interface IAfterQueryTask
{
    Task ExecuteAsync(CustomerSettingModel customerSetting);
}
