namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels.GroupType161;

public class BetsettingItem
{
    public decimal MinBet { get; set; }
    
    public decimal MaxBet { get; set; }

    public decimal MaxPerMatch { get; set; }

    public BetsettingItem Upline { get; set; }
}
