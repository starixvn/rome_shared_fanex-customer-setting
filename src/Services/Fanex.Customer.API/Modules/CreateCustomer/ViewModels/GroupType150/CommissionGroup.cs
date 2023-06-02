namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;

public class CommissionGroup : GroupItemDefault
{
    private const string CommissionGroupXmlFormat = "<R {0} Discount=\"{1}\" />";

    public decimal Discount { get; set; }

    public decimal DiscountMember { get; set; }

    public decimal Step { get; set; }

    public override string ToXml()
    => string.Format(CommissionGroupXmlFormat, base.ToXml(), Discount);

}
