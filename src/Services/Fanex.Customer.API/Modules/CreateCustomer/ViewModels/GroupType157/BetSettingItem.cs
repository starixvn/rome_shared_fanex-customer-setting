namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;

public class BetSettingItem : GroupItemDefault
{
    private const string BetSettingXmlFormat = "<R {0} MinBet=\"{1}\" MaxBet=\"{2}\" MPM=\"{3}\"/>";

    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    public decimal MaxPerMatch { get; set; }

    public override string ToXml()
    => string.Format(BetSettingXmlFormat,
        base.ToXml(),
        MinBet,
        MaxBet,
        MaxPerMatch);
}
