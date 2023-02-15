namespace Alpha.CloneAccount.Models;

public class CustomerPTInfo
{
    public CustomerPTInfo()
    {
    }

    public CustomerPTInfo(
        int custId,
        byte custLevel,
        string suffix,
        decimal maxCredit)
    {
        CustId = custId;
        CustLevel = custLevel;
        Suffix = suffix;
        MaxCredit = maxCredit;
    }

    public int CustId { get; set; }

    public byte CustLevel { get; set; }

    public string Suffix { get; }

    public decimal MaxCredit { get; }
}

