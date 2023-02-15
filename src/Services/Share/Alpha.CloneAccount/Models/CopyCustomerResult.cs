namespace Alpha.CloneAccount.Models;

public class CopyCustomerResult : Result
{
    public CopyCustomerResult(
        int custId,
        string custName,
        string password,
        int custLevel,
        string sourceUsername,
        decimal credit,
        decimal maxCredit,
        int code,
        string message) : base(code, message)
    {
        CustId = custId;
        CustName = custName;
        Password = password;
        CustLevel = custLevel;
        SourceUsername = sourceUsername;
        Credit = credit;
        MaxCredit = maxCredit;
    }

    public int CustId { get; }

    public string CustName { get; }

    public string Password { get; }

    public int CustLevel { get; }

    public string SourceUsername { get; }

    public decimal Credit { get; }

    public decimal MaxCredit { get; }
}
