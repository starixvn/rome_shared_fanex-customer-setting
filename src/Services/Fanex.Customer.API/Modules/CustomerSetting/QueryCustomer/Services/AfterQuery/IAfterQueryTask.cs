using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public interface IAfterQueryTask
{
    int GroupTypeId { get; }
    Task ExecuteAsync(CustomerSettingModel customerSetting, AfterQueryContext context);
}

public class AfterQueryContext
{
    public string Language { get; set; }
    public int CustId { get; set; }
    public byte SettingStatus { get; set; }
    public string Site { get; set; }
    public int Currency { get; set; }
}