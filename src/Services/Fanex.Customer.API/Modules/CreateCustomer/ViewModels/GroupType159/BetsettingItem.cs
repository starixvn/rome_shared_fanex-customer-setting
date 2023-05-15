namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType159;

public class BetsettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    public decimal MaxPerNumber { get; set; }

    public BetsettingItem Upline { get; set; }
}
