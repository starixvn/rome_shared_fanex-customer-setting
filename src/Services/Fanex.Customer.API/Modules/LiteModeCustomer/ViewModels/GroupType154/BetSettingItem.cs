namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels.GroupType154;

public class BetSettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }
    
    public decimal MaxPerMatch { get; set; }

    public decimal? MaxPerBall { get; set; }

    public BetSettingItem Upline { get; set; }
}
