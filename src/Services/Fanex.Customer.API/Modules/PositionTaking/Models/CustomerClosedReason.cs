using System.Globalization;
using Fanex.Customer.API.Modules.PositionTaking.Resources;

namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public sealed class CustomerClosedReason
{
    private const byte ByAdmin = 1; // purple: #b3a2c7
    private const byte ByUpline = 2; // orange: #f79646
    private const byte ByArchive = 4; // dark grey hi-light: #969696

    public CustomerClosedReason(object closedReason, string language)
    {
        if (closedReason != null && closedReason != DBNull.Value)
        {
            AssignParameters(Convert.ToByte(closedReason), language);
        }
    }

    public string ClassName { get; private set; }

    public string ClosedReason { get; private set; }

    private void AssignParameters(byte closedReason, string language)
    {
        ClassName = "closed-";

        switch (closedReason)
        {
            case ByAdmin:
                ClassName += "byAdmin";
                ClosedReason = Lang.ResourceManager.GetString("ClosedByAdmin", new CultureInfo(language));
                break;

            case ByUpline:
                ClassName += "byUpline";
                ClosedReason = Lang.ResourceManager.GetString("ClosedByUpline", new CultureInfo(language));
                break;

            case ByArchive:
                ClassName += "byArchive";
                ClosedReason = Lang.ResourceManager.GetString("ClosedByArchive", new CultureInfo(language));
                break;

            default:
                ClassName = string.Empty;
                ClosedReason = string.Empty;
                break;
        }
    }
}