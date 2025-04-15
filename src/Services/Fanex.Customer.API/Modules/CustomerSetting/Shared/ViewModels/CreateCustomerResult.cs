using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class CreateCustomerResult : CustomerSettingResult
{
    public CreateCustomerResult(int newCustId, int code = 0, string message = "")
        : base(code, message, null)
    {
        NewCustIds = new List<int> { newCustId };
    }

    public CreateCustomerResult(List<int> newCustIds, int code = 0, string message = "")
        : base(code, message, null)
    {
        NewCustIds = newCustIds ?? new List<int>();
    }

    public CreateCustomerResult(int newCustId, int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message, subMessages)
    {
        NewCustIds = new List<int> { newCustId };
    }


    public CreateCustomerResult(List<int> newCustIds, int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message, subMessages)
    {
        NewCustIds = newCustIds ?? new List<int>();
    }

    public CreateCustomerResult(int newCustId, int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null, List<string> newUserNames = null, List<string> availableDomains = null)
        : base(code, message, subMessages)
    {
        NewCustIds = new List<int> { newCustId };
        NewUserNames = newUserNames;
        AvailableAlphaDomain = availableDomains;
    }

    public CreateCustomerResult(List<int> newCustIds, int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null, List<string> newUserNames = null, List<string> availableDomains = null)
        : base(code, message, subMessages)
    {
        NewCustIds = newCustIds ?? new List<int>();
        NewUserNames = newUserNames;
        AvailableAlphaDomain = availableDomains;
    }

    public List<int> NewCustIds { get; }

    public List<string> NewUserNames { get; set; }

    public List<string> AvailableAlphaDomain { get; set; }
}

public class CustomerSettingResult : Result
{
    public CustomerSettingResult(int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message)
    {
        SubMessages = subMessages;
    }

    public IEnumerable<SubMessageContent> SubMessages { get; set; }
}