namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class QuerySuperRequest : BaseQueryRequest
{
    public int Currency { get; set; }

    public string Site { get; set; }

    public string SiteLabel { get; set; }
}
