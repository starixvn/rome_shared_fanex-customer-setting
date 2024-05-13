namespace Fanex.Customer.API.Modules.Customer.Models;

public class CurrentWinLossAmount
{
    public int SportType { get; set; }

    public string SportTypeName { get; set; }

    public decimal CurrentAmount { get; set; }

    public decimal WinPercent { get; set; }

    public decimal MaxWinAmount { get; set; }

    public decimal LossPercent { get; set; }

    public decimal MaxLossAmount { get; set; }
}
