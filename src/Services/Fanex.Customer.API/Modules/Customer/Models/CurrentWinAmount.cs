namespace Fanex.Customer.API.Modules.Customer.Models;

public class CurrentWinAmount
{
    public int SportType { get; set; }
    public string SportTypeName { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal WinPercent { get; set; }
    public decimal MaxWinAmount { get; set; }
}
