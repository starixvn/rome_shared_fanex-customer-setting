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
    public bool IsValid() => CustId > 0 && UserId > 0 && SubAccId >= 0;
}