namespace Fanex.Customer.API.Modules.Customer.Requests.CashService;

public class RemoveMemberBankInfoRequest
{
    public int ActorId { get; set; }

    public int CustId { get; set; }

    public int BankAccountMemberID { get; set; }

    public string IP { get; set; }

    public string Language { get; set; } = "en-US";

    public int SubAccID { get; set; }
}