namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;

public class LossLimitSettingItem : GroupItemDefault
{
    public decimal LossLimit { get; set; }

    public decimal MaxLossRange { get; set; }

    public decimal MinLossRange { get; set; }

    public bool IsAutoReset { get; set; }

    public bool IsAutoResetUpline { get; set; }
}
