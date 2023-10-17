namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class PTTemplate
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsDefault { get; set; }

    public IEnumerable<PTTemplateDetail> Settings { get; set; }
}