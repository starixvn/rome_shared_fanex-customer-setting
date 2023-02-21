namespace Fanex.Customer.API.Modules.SecurityCode;

public class VerifySecurityCodeParam
{
    public int CustId { get; set; }

    public int SubAccId { get; set; }

    public string SecurityCode { get; set; } = string.Empty;

    public string Language { get; set; } = "en-US";

    public string IP { get; set; } = string.Empty;

    public bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(SecurityCode);

    public bool IsSubAccount() => SubAccId > 0;
}