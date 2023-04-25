namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels.GroupType155;

public class WinlossLimitSettingItem : GroupItemDefault
{
    public decimal WinLimit { get; set; }

    // [3, 5, 10, 0] 0: Unlimited
    public decimal MaxWinRange { get; set; }

    public decimal LossLimit { get; set; }

    public decimal MaxLossRange { get; set; }

    public decimal MinLossRange { get; set; }

    public bool IsAutoReset { get; set; }

    public bool IsAutoResetUpline { get; set; }
}
