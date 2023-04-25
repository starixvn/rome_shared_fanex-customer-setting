using Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels.GroupType151;

public class BetSettingItem : GroupItemDefault
{
    public BetSettingItem() : this(0, 0, null) 
    { }

    public BetSettingItem(int sporttypeid, int bettypeid, string DisplayName) 
    : base(sporttypeid, bettypeid, DisplayName)
    { }

    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    public decimal MaxPerMatch { get; set; }

    public decimal MaxPayoutPerMatch { get; set; }

    public BetSettingItem Upline { get; set; }
}
