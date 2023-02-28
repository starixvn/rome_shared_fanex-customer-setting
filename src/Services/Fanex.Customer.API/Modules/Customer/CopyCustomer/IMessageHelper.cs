using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public interface IMessageHelper
{
    void SetErrorCodeAndErrorMessage(string errorCode, string errorMessage);

    IList<Result> GetErrors();
}