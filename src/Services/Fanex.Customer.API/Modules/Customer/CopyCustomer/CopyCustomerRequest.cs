namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class CopyCustomerRequest
{
    public int ActorId { get; set; }

    public int ActorSubAccId { get; set; }

    public string ActorSubAccName { get; set; }

    public byte ActorUserLevel { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public decimal Credit { get; set; }

    public decimal MaxCredit { get; set; }

    public int SourceCustId { get; set; }

    public string SourceUsername { get; set; }

    public int NumberOfAccounts { get; set; }

    public string Language { get; set; }

    public string IP { get; set; }

    public byte SystemId { get; set; }

    public bool IsQrCode { get; set; }

    public bool IsValid()
        => ActorId > 0
        && ActorSubAccId >= 0
        && 1 < ActorUserLevel && ActorUserLevel < 5
        && !string.IsNullOrWhiteSpace(UserName)
        && !string.IsNullOrWhiteSpace(Password)
        && 0 < NumberOfAccounts && NumberOfAccounts <= 10
        && Credit >= 0
        && MaxCredit >= 0
        && SourceCustId > 0
        && SystemId > 0;
}