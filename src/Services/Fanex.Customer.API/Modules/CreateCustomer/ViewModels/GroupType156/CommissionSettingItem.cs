namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;

public class CommissionSettingItem : GroupItemDefault
{
    private const string CommissionXmlFormat = "<R {0} discount=\"{1}\" />";

    public decimal Discount { get; set; }

    public CommissionSettingItem Upline { get; set; }

    public override string ToXml()
    => string.Format(CommissionXmlFormat, 
        base.ToXml(), 
        Discount);
}
