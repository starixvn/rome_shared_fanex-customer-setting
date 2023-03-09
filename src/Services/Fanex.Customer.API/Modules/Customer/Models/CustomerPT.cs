namespace Fanex.Customer.API.Modules.Customer.Models;

public class CustomerPT
{
    public CustomerPT()
    {

    }

    public CustomerPT(string suffix, decimal maxCredit)
    {
        Suffix = suffix;
        MaxCredit = maxCredit;
    }

    public string Suffix { get; }

    public decimal MaxCredit { get; }
}