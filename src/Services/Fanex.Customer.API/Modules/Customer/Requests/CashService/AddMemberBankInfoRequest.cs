namespace Fanex.Customer.API.Modules.Customer.Requests.CashService;

public class AddMemberBankInfoRequest
{
    public int ActorId { get; set; }
    public int CustId { get; set; }
    public int MemberBankId { get; set; }
    public int MemberBankAccountNumber { get; set; }


    public int AgentBankAccountNumber { get; set; }
    
    public string IP { get; set; }
    public string Language { get; set; } = "en-US";
    public int SubAccID { get; set; }
}

