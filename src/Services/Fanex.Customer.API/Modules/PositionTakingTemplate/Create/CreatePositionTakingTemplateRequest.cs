namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePositionTakingTemplateRequest
{
    public int CustId { get; set; }

    public PTTemplate Template { get; set; }

    public string Language { get; set; } = "E";

    public string IP { get; set; } = "";

    public bool IsValid() => CustId > 0;
}