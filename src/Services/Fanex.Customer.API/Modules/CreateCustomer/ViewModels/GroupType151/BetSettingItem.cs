using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;

public class BetSettingItem : GroupItemDefault
{
    private const string BetSettingXmlFormat = "<R {0} minbet=\"{1}\" maxbet=\"{2}\" mpm=\"{3}\" {4}/>";

    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    public decimal MaxPerMatch { get; set; }

    public decimal? MaxPayoutPerMatch { get; set; }

    public BetSettingItem Upline { get; set; }

    public override string ToXml()
    => string.Format(BetSettingXmlFormat, 
        base.ToXml(), 
        MinBet, 
        MaxBet, 
        MaxPerMatch, 
        MaxPayoutPerMatch.HasValue ? $"maxpayout=\"{MaxPayoutPerMatch.Value}\"" : string.Empty);
}
