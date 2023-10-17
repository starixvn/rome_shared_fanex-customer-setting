namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class PTTemplateDetail
{
    public byte GroupId { get; set; }

    public byte SubGroupId { get; set; }

    public decimal PT { get; set; }

    public string ToXml()
        => $"<r group=\"{GroupId}\" sub=\"{SubGroupId}\" pt=\"{PT}\" />";
}