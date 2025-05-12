namespace Fanex.Customer.API.Modules.Customer.Models;

public class CurrentPotentialWinLimitAmount
{
    public int SportType { get; set; }

    public string SportTypeName { get; set; }

    public decimal AccumToWin { get; set; }    

    public decimal AccumWinloss { get; set; }

    public decimal AccumAmount => AccumToWin + AccumWinloss;
    
    public decimal AccumPercent(decimal totalAccumAmount) => AccumAmount / totalAccumAmount;
}
