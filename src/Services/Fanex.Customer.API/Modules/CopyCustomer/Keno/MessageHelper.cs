using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.CopyCustomer.Keno;

public interface IMessageHelper
{
    void SetErrorCodeAndErrorMessage(string errorCode, string errorMessage);

    IList<Result> GetErrors();
}

public class MessageHelper : IMessageHelper
{
    private const string Separator = "\a";
    private const int SuccessfulCode = 0;
    private const int DatabaseResponseIsNotValidCode = 1001;
    private const string MessageFormat = "Msg_{0}";
    private string _errorCode = string.Empty;
    private string _errorMsg = string.Empty;
    private IList<Result> _errorResults;

    public MessageHelper()
    {
    }

    public MessageHelper(string errorCode, string errorMessage)
        : this()
    {
        SetErrorCodeAndErrorMessage(errorCode, errorMessage);
    }

    internal IList<Result> ErrorResults
    {
        get { return _errorResults; }
    }

    public void SetErrorCodeAndErrorMessage(string errorCode, string errorMessage)
    {
        _errorResults = new List<Result>();
        _errorCode = errorCode;
        _errorMsg = errorMessage;
    }

    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <returns>The list of errors.</returns>
    public IList<Result> GetErrors()
    {
        if (DataIsNotChanged())
        {
            AddMessageWhenDataIsNotChanged();
        }
        else if (DatabaseResponseIsNotValid())
        {
            AddMessageInCaseDatabaseResponseIsNotValid();
        }
        else
        {
            AddMessageWhenDataIsUpdated();
        }

        return _errorResults;
    }

    internal static string GetResourceFromErrorCode(string errorCode)
    {
        string result = Messages.ResourceManager.GetString(string.Format(MessageFormat, errorCode));
        return !string.IsNullOrEmpty(result) ? result : errorCode;
    }

    internal static bool IfErrorCodeIsValid(string errorCode)
    {
        return int.TryParse(errorCode, out int formattedErrorCode);
    }

    internal static string UpdateErrorCodeBecomesSuccessfulCodeWhenItIsEmpty(string[] errorCodes, int index)
    {
        if (index < errorCodes.Length - 1 && string.IsNullOrEmpty(errorCodes[index]))
        {
            return SuccessfulCode.ToString();
        }

        return errorCodes[index];
    }

    internal bool DataIsNotChanged()
    {
        return string.IsNullOrEmpty(this._errorCode);
    }

    internal void AddMessageWhenDataIsUpdated()
    {
        var errorCodes = _errorCode.Split(new[] { Separator }, StringSplitOptions.None);
        var errorMsgs = _errorMsg.Split(new[] { Separator }, StringSplitOptions.None);

        for (int i = 0; i < errorCodes.Length; i++)
        {
            errorCodes[i] = UpdateErrorCodeBecomesSuccessfulCodeWhenItIsEmpty(errorCodes, i);
            AddErrorMessage(errorCodes[i], errorMsgs[i]);
        }
    }

    internal void AddErrorMessage(string errorCode, string errorMsg)
    {
        if (IfErrorCodeIsValid(errorCode))
        {
            _errorResults.Add(new Result(Convert.ToInt32(errorCode), !string.IsNullOrEmpty(errorMsg) ? errorMsg : GetResourceFromErrorCode(errorCode)));
        }
    }

    internal void AddMessageWhenDataIsNotChanged()
    {
        _errorResults.Add(new Result(SuccessfulCode, GetResourceFromErrorCode(SuccessfulCode.ToString())));
    }

    /// <summary>
    /// Database response is not valid.
    /// </summary>
    /// <returns>
    /// Whether the database response is valid or not.
    /// </returns>
    internal bool DatabaseResponseIsNotValid()
    {
        var errorCodes = _errorCode.Split(new[] { Separator }, StringSplitOptions.None);
        var errorMsgs = _errorMsg.Split(new[] { Separator }, StringSplitOptions.None);

        if (errorCodes.Length != errorMsgs.Length)
        {
            return true;
        }

        return false;
    }

    internal void AddMessageInCaseDatabaseResponseIsNotValid()
    {
        _errorResults.Add(new Result(DatabaseResponseIsNotValidCode, GetResourceFromErrorCode(DatabaseResponseIsNotValidCode.ToString())));
    }
}