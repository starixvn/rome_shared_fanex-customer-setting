namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePTTemplateResult : PTTemplateResult
{
    public CreatePTTemplateResult(int templateId, int code = 0, string message = "", IEnumerable<string> subMessages = null)
        : base(code, message, subMessages)
    {
        TemplateId = templateId;
    }

    public int TemplateId { get; }
}