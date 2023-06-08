namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType159;

public class BetsettingItem : GroupItemDefault
{
    private const string BetSettingXmlFormat = "<R {0} MinBet=\"{1}\" MaxBet=\"{2}\" MaxPerNumber=\"{3}\" />";

    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    public decimal MaxPerNumber { get; set; }

    public BetsettingItem Upline { get; set; }

    public override string ToXml()
    => string.Format(BetSettingXmlFormat, 
        base.ToXml(), 
        MinBet, 
        MaxBet, 
        MaxPerNumber);
}
