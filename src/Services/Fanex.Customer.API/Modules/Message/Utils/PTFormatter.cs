namespace Fanex.Customer.API.Modules.Message.Utils;

internal static class PTFormatter
{
    private const byte PTDivider = 100;

    public static string FormatPT(decimal value) => (value / PTDivider).ToString("G29");
}