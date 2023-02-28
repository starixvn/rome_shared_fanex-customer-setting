using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class CopyMultipleCustomerResult : Result
{
    public CopyMultipleCustomerResult(IEnumerable<string> usernames, int code = 0, string message = "")
        : base(code, message)
    {
        Usernames = usernames;
    }

    public IEnumerable<string> Usernames { get; }
}