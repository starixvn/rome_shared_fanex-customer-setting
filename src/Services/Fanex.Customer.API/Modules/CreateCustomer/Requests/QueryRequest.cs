namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class QueryRequest
{
    public int UplineCustId { get; set; }

    public int UplineRoleId { get; set; }

    public string UplineSite { get; set; }

    public int UplineCurrency { get; set; }

    public string Language { get; set; }
}
