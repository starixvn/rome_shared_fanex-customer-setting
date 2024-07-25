namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class BaseQueryRequest
{
    public string Language { get; set; }

    public int? ProductGroupId { get; set; }

    public byte SystemId { get; set; }
}
