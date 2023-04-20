using Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels.GroupType151;

public class BetSettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }
    
    public decimal MaxBet { get; set; }

    public decimal MaxPerMatch { get; set; }

    public decimal MaxPayoutPerMatch { get; set; }

    public BetSettingItem Upline { get; set; }
}
