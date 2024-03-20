namespace Fanex.Customer.API.Modules.Customer.Models;

public class CustomerInfo
{
    public string UserName { get; init; }

    public string NickName { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string Phone { get; init; }

    public string MobilePhone { get; init; }

    public string Fax { get; init; }

    public int CurrencyId { get; init; }

    public string CurrencyName { get; init; }

    public string Site { get; init; }

    public string SiteLabel { get; init; }

    public byte RoleId { get; init; }

    public decimal Credit { get; init; }

    public decimal MaxCredit { get; init; }
}

