namespace Fanex.Customer.API.Modules.Customer.SearchCustomer;

public class SearchCustomerRequest
{
    public string Query { get; set; }

    public int CustId { get; set; }

    public int IsSubAccount { get; set; }

    public int ExcludeMember { get; set; }

    public int DirectDownLine { get; set; }

    public int IsAccInfo { get; set; }

    public bool IsInternalAgent { get; set; }

    public static ValueTask<SearchCustomerRequest> BindAsync(HttpContext context)
    {
        var result = new SearchCustomerRequest
        {
            Query = context.Request.Query[nameof(Query)],
            CustId = int.TryParse(context.Request.Query[nameof(CustId)], out var custId) ? custId : 0,
            IsSubAccount = int.TryParse(context.Request.Query[nameof(IsSubAccount)], out var isSucAccount) ? isSucAccount : 0,
            ExcludeMember = int.TryParse(context.Request.Query[nameof(ExcludeMember)], out var excludeMember) ? excludeMember : 0,
            DirectDownLine = int.TryParse(context.Request.Query[nameof(DirectDownLine)], out var directDownline) ? directDownline : 0,
            IsAccInfo = int.TryParse(context.Request.Query[nameof(IsAccInfo)], out var isAccInfo) ? isAccInfo : 0,
            IsInternalAgent = bool.TryParse(context.Request.Query[nameof(IsInternalAgent)], out var isInternalAgent) && isInternalAgent,
        };

        return ValueTask.FromResult(result);
    }

    public bool IsValid()
        => CustId > 0;
}