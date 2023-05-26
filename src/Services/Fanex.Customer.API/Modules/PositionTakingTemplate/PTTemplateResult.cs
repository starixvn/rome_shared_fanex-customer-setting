using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class PTTemplateResult : Result
{
    public PTTemplateResult(int code = 0, string message = "", IEnumerable<string> subMessages = null)
        : base(code, message)
    {
        SubMessages = subMessages;
    }

    public IEnumerable<string> SubMessages { get; }
}