using System.Globalization;
using Fanex.Customer.API.Modules.PositionTaking.Resources;

namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public sealed class CustomerLockedAccess
{
    private const byte None = 0;
    private const byte FalseOTPSC = 1; // blue: #86c5e4
    private const byte FalsePassword = 2; // gray: #ccc7ba

    public CustomerLockedAccess(byte locked, string language)
    {
        Locked = locked;
        AssignParameters(language);
    }

    public byte Locked { get; }

    public bool IsLocked => Locked != None;

    public string ClassName { get; private set; }

    public string UnlockConfirmMessage { get; private set; }

    private void AssignParameters(string language)
    {
        if (Locked == FalseOTPSC)
        {
            ClassName = "locked-falseOTPSC";
            UnlockConfirmMessage = Lang.ResourceManager.GetString("FalseOTPSCUnlockConfirm", new CultureInfo(language));
        }
        else if (Locked == FalsePassword)
        {
            ClassName = "locked-falsePassword";
            UnlockConfirmMessage = Lang.ResourceManager.GetString("FalsePasswordUnlockConfirm", new CultureInfo(language));
        }
    }
}