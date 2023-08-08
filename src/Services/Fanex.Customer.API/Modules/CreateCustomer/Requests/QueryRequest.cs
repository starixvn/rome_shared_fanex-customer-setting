namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class QueryRequest
{
    public int UplineCustId { get; set; }

    public byte UplineRoleId { get; set; }

    public string Language { get; set; }
}
