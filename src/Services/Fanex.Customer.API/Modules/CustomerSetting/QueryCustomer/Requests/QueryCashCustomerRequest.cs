namespace Fanex.Customer.API.Modules.QueryCustomer.Requests;

public class QueryCashCustomerRequest : BaseQueryRequest
{
    public int CustId { get; set; }

    public byte RoleId { get; set; }

    public bool IsGetUpline { get; set; }

    public byte? UplineRoleId { get; set; }

    public string SiteLabel { get; set; }
}
