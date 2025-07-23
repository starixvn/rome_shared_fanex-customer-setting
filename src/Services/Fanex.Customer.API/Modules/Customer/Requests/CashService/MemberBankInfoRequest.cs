namespace Fanex.Customer.API.Modules.Customer.Requests.CashService;

public class BaseMemberBankInfoRequest
{
    public int ActorId { get; set; }
    public int CustId { get; set; }
    public string IP { get; set; }
    public string Language { get; set; } = "en-US";
    public int SubAccId { get; set; }
}

public class AddMemberBankInfoRequest : BaseMemberBankInfoRequest
{
    public int MemberBankId { get; set; }
    public string MemberBankAccountNumber { get; set; }
    public string AgentBankAccountNumber { get; set; }
    public int AgentBankId { get; set; }
}

public class RemoveMemberBankInfoRequest : BaseMemberBankInfoRequest
{
    public int MemberBankAccountId { get; set; }
}

public class UpdateMemberBankInfoRequest : AddMemberBankInfoRequest
{
    public int MemberBankAccountId { get; set; }
}