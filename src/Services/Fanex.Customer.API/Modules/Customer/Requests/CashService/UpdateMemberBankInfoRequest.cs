namespace Fanex.Customer.API.Modules.Customer.Requests.CashService;
public class UpdateMemberBankInfoRequest : AddMemberBankInfoRequest
{
    public int BankAccountMemberID { get; set; }
}