namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;

public class WinlossLimitSettingItem : GroupItemDefault
{
    private const string WinlossLimitXmlFormat = "<R {0} MaxWinLimit=\"{1}\" MaxLossLimit=\"{2}\" IsAutoReset=\"{3}\" />";

    public decimal WinLimit { get; set; }

    // [3, 5, 10, 0] 0: Unlimited
    public decimal MaxWinRange { get; set; }

    public decimal LossLimit { get; set; }

    public decimal MaxLossRange { get; set; }

    public decimal MinLossRange { get; set; }

    public bool IsAutoReset { get; set; }

    public bool IsAutoResetUpline { get; set; }

    public override string ToXml()
    => string.Format(WinlossLimitXmlFormat, 
        base.ToXml(), 
        WinLimit, 
        LossLimit, 
        IsAutoReset);
}
