namespace Fanex.Customer.API.Modules.Customer.Requests.CashService;

public class AddMemberBankInfoRequest
{
    public int ActorId { get; set; }
    public int CustId { get; set; }
    public int MemberBankId { get; set; }
    public string MemberBankAccountNumber { get; set; }


    public string AgentBankAccountNumber { get; set; }
    public int AgentBankId { get; set; }
    
    public string IP { get; set; }
    public string Language { get; set; } = "en-US";
    public int SubAccID { get; set; }
}

