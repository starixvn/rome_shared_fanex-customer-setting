namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public sealed class CustomerClosedReason
{
    private const byte ByAdmin = 1; // purple: #b3a2c7
    private const byte ByUpline = 2; // orange: #f79646
    private const byte ByArchive = 4; // dark grey hi-light: #969696

    public CustomerClosedReason(object closedReason)
    {
        if (closedReason != null && closedReason != DBNull.Value)
        {
            AssignParameters(Convert.ToByte(closedReason));
        }
    }

    public CustomerClosedReason(byte closedReason)
    {
        AssignParameters(closedReason);
    }

    public string ClassName { get; private set; }

    public string ClosedReason { get; private set; }

    private void AssignParameters(byte closedReason)
    {
        ClassName = "closed-";

        switch (closedReason)
        {
            case ByAdmin:
                ClassName += "byAdmin";
                // TODO:ClosedReason = CoreLang.ClosedByAdmin;
                break;

            case ByUpline:
                ClassName += "byUpline";
                // TODO:ClosedReason = CoreLang.ClosedByUpline;
                break;

            case ByArchive:
                ClassName += "byArchive";
                // TODO:ClosedReason = CoreLang.ClosedByArchive;
                break;

            default:
                ClassName = string.Empty;
                ClosedReason = string.Empty;
                break;
        }
    }
}