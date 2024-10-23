namespace Fanex.Customer.API.Modules.QueryCustomer.Requests;

public class QueryPTTemplateRequest
{
    public int CustId { get; set; }

    public int RoleId { get; set; }

    public string Site { get; set; }

    public int Currency { get; set; }

    public string Language { get; set; }

    public byte SystemId { get; set; }
}
