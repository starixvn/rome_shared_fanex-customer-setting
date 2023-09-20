using System.Text.RegularExpressions;

namespace Fanex.Customer.API.Shared.Extensions;

public static class StringExtensions
{
    static Regex htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
    public static string RemoveHTMLTags(this string value)
    {
        return htmlRegex.Replace(value, string.Empty);
    }
}