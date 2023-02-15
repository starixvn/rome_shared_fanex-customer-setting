
using Alpha.CloneAccount.Models;

namespace Alpha.CloneAccount.Helpers;

public interface IMessageHelper
{
    void SetErrorCodeAndErrorMessage(string errorCode, string errorMessage);

    IList<Result> GetErrors();
}