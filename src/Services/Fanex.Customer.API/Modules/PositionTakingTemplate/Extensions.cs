using System.Text;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public static class PositionTakingTemplateExtensions
{
    public static string BuildXMLSettings(this IEnumerable<PTTemplateDetail> settings)
    {
        var builder = new StringBuilder();

        foreach (var item in settings)
        {
            builder.Append(item.ToXml());
        }

        return $"<Root>{builder}</Root>";
    }
}