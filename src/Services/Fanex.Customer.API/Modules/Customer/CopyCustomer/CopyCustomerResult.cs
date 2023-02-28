using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class CopyCustomerResult : Result
{
    public CopyCustomerResult(int custId, int code = 0, string message = "")
        : base(code, message)
    {
        CustId = custId;
    }

    public int CustId { get; }
}