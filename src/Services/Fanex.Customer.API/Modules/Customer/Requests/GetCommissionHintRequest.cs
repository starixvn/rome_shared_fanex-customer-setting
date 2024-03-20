namespace Fanex.Customer.API.Modules.Customer.Requests;

public class GetCommissionHintRequest
{
    public int CustId { get; set; }
    
    public string Site { get; set; }
    
    public string Language { get; set; }
}
