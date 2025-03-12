using System.Text.RegularExpressions;

namespace Fanex.Customer.API.Shared.Extensions;

public static class StringExtensions
{
    public static bool ContainsHTML(this string checkString)
        => Regex.IsMatch(checkString, "<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+");
}