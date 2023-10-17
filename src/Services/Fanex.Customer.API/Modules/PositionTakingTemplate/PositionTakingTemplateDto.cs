namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class PositionTakingTemplateDto
{
    public int TemplateId { get; init; }

    public string TemplateName { get; init; }

    public bool IsDefault { get; init; }

    public byte GroupId { get; init; }

    public byte SubGroupId { get; init; }

    public decimal PT { get; init; }
}