namespace Fanex.Customer.API.Modules.Customer.Models;

public class CreditInfo
{
    public decimal Credit { get; init; }

    public decimal MaxCredit { get; init; }

    public decimal? MemberMaxCredit { get; init; }
}