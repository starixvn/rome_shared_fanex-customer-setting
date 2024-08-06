namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class CreateNickNameRequest
{
    public int CustId { get; set; }

    public string NickName { get; set; }

    public string Language { get; set; }

    public string Ip { get; set; }
}
