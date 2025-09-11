using Fanex.Customer.API.Shared.Extensions;

namespace Fanex.Customer.API.Modules.Customer.Requests;

public record UpdateCustomerInfoRequest(
    int CustId,
    string FirstName,
    string LastName,
    string Phone,
    string MobilePhone,
    string Email,
    string Fax,
    int UserId,
    int SubAccId,
    string IP,
    string Language)
{
    public bool IsValid() => CustId > 0 && UserId > 0 && SubAccId >= 0 && ValidateUpdateCustomerInfoRequest();

    private bool ValidateUpdateCustomerInfoRequest()
    {
        if ((!string.IsNullOrWhiteSpace(FirstName) && FirstName.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(LastName) && LastName.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(Phone) && Phone.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(MobilePhone) && MobilePhone.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(Fax) && Fax.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(Email) && Email.ContainsHTML()))
        {
            return false;
        }

        return true;
    }
}