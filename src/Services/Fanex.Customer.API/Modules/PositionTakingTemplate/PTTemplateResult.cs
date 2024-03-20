using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class PTTemplateResult : Result
{
    public PTTemplateResult(int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message)
    {
        SubMessages = subMessages;
    }

    public IEnumerable<SubMessageContent> SubMessages { get; }
}