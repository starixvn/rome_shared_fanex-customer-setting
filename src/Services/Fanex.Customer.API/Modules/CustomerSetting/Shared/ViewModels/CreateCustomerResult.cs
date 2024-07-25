using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class CreateCustomerResult : CustomerSettingResult
{
    public CreateCustomerResult(int newCustId, int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message, subMessages)
    {
        NewCustId = newCustId;
    }

    public int NewCustId { get; }
    
    public List<string> AvailableAlphaDomain { get; set; }
}

public class CustomerSettingResult : Result
{
    public CustomerSettingResult(int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message)
    {
        SubMessages = subMessages;
    }

    public IEnumerable<SubMessageContent> SubMessages { get; set;}
}