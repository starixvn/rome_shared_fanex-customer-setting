namespace Fanex.Customer.API.Modules.Customer.Requests;

public class GetCurrentWinLossAmountRequest
{
    public int CustId { get; set; }
    
    public int SportType { get; set; }

    public int BetType { get; set; }
    
    public string Language { get; set; }
}
