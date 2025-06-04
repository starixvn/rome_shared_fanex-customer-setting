namespace Fanex.Customer.API.Modules.Customer.Requests.CashService;

public class UpdateCashServiceStatusRequest
{
    public int CustId { get; set; }
    public byte ServiceType { get; set; } // 0 - Credit, 1 - Cash
    public int SubAccId { get; set; }
    public string IP { get; set; }
    public string Language { get; set; } = "en-US";
    public string Site { get; set; }
}
