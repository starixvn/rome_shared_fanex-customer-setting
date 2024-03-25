using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CopyCustomerRequest
{
    public bool IsQrCode { get; set; }

    public int UplineId { get; set; }

    public int ActorId { get; set; }

    public int ActorSubAccId { get; set; }

    public string ActorUsername { get; set; }

    public byte CustLevel { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public decimal Credit { get; set; }

    public decimal MaxCredit { get; set; }

    public int SourceCustId { get; set; }

    public string Language { get; set; }

    public string LanguageKey => LanguageUtils.ToLanguageKey(Language);

    public string IP { get; set; }

    public byte SystemId { get; set; }

    public string Site { get; set; }

    public bool IsInternal { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string MobilePhone { get; set; } = string.Empty;

    public string Fax { get; set; } = string.Empty;

    public virtual bool IsValid()
        => ActorId > 0
        && ActorSubAccId >= 0
        && UplineId >= 0
        && 1 <= CustLevel && CustLevel < 5
        && !string.IsNullOrWhiteSpace(UserName)
        && !string.IsNullOrWhiteSpace(Password)
        && Credit >= 0
        && MaxCredit >= 0
        && SourceCustId > 0
        && SystemId > 0;
}