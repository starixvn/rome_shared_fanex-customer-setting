namespace Fanex.Customer.API.Modules.Customer.Models;

public class MemberBankInfo
{
    // Member bank information
    public int MemberBankAccountID { get; set; }
    public string MemberBankAccountNumber { get; set; }
    public int MemberBankID { get; set; }
    public string MemberBankCode { get; set; }

    // Agent bank information
    public int AgentBankAccountID { get; set; }
    public string AgentBankAccountNumber { get; set; }
    public int AgentBankID { get; set; }
    public string AgentBankCode { get; set; }
    public string AgentBankAccountStatus { get; set; }
}