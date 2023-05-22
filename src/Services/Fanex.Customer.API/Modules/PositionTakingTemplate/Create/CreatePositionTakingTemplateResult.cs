using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePositionTakingTemplateResult : Result
{
    public CreatePositionTakingTemplateResult(int code, string message, int templateId) : base(code, message)
    {
        TemplateId = templateId;
    }

    public int TemplateId { get; }
}