using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class CreateCustomerResult : Result
{
    public CreateCustomerResult(int code = 0, string message = "", IEnumerable<string> subMessages = null)
        : base(code, message)
    {
        SubMessages = subMessages;
    }
    
    public IEnumerable<string> SubMessages { get; }
}