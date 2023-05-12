using System.ComponentModel.DataAnnotations;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePositionTakingTemplateRequest
{
    [Required]
    public int CustId { get; set; }

    [Required]
    public string TemplateName { get; set; }

    public bool IsDefault { get; set; } = false;

    [Required]
    public IEnumerable<PTTemplateDetail> Settings { get; set; }

    public string Language { get; set; } = "E";

    public string IP { get; set; } = "";
}