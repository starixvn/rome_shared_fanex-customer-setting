namespace Fanex.Customer.API.Modules.Customer.Models;

public class CurrentWinLossAmount : CurrentWinAmount
{
    public decimal LossPercent { get; set; }
    public decimal MaxLossAmount { get; set; }
}
