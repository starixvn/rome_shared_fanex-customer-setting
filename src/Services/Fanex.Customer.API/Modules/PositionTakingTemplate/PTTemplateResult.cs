using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class PTTemplateResult : Result
{
    public PTTemplateResult(int templateId, int code = 0, string message = "", IEnumerable<string> subMessages = null)
        : base(code, message)
    {
        TemplateId = templateId;
        SubMessages = subMessages;
    }

    public int TemplateId { get; }

    public IEnumerable<string> SubMessages { get; }
}