using Fanex.Customer.API.Shared.Constants;

namespace Fanex.Customer.API.Modules.QueryCustomer.Requests;

public class QueryCashMemberRequest : BaseQueryRequest
{
    public int CustId { get; set; }

    public byte RoleId { get; set; }

    public bool IsGetUpline { get; set; }

    public byte? UplineRoleId { get; set; }

    public string SiteLabel { get; set; }
}
