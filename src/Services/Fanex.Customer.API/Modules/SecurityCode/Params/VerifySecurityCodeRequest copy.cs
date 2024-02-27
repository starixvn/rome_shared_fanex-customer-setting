namespace Fanex.Customer.API.Modules.SecurityCode.Params;

public class CheckSecurityCodeRequest : VerifySecurityCodeRequest
{
    public string Salt { get; set; }
}