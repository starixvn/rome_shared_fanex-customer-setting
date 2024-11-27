namespace Fanex.Customer.API.Modules.QueryCustomer.Requests;

public class QueryRequest : BaseQueryRequest
{
    public int CustId { get; set; }

    public byte RoleId { get; set; }

    public bool IsGetUpline { get; set; }

    public byte? UplineRoleId { get; set; }

    public string SiteLabel { get; set; }

    public string GroupName { get; set; }

    public bool IsMultipleMode { get; set; }

    public bool IsCopy { get; set; }
}
