namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;

public class LossLimitSettingItem : GroupItemDefault
{
    private const string CommissionXmlFormat = "<R {0} MaxLossLimit=\"{1}\" IsAutoReset=\"{2}\" />";

    public decimal LossLimit { get; set; }

    public decimal MaxLossRange { get; set; }

    public decimal MinLossRange { get; set; }

    public bool IsAutoReset { get; set; }

    public bool IsAutoResetUpline { get; set; }

    public override string ToXml()
    => string.Format(CommissionXmlFormat, 
        base.ToXml(), 
        LossLimit,
        IsAutoReset);
}
