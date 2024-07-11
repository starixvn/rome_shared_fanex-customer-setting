namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public sealed class CustomerLockedAccess
{
    private const byte None = 0;
    private const byte FalseOTPSC = 1; // blue: #86c5e4
    private const byte FalsePassword = 2; // gray: #ccc7ba

    public CustomerLockedAccess(byte locked)
    {
        Locked = locked;
        AssignParameters();
    }

    public byte Locked { get; }

    public bool IsLocked => Locked != None;

    public string ClassName { get; private set; }

    public string UnlockConfirmMessage { get; private set; }

    private void AssignParameters()
    {
        if (Locked == FalseOTPSC)
        {
            ClassName = "locked-falseOTPSC";
            //TODO:UnlockConfirmMessage = CoreLang.FalseOTPSCUnlockConfirm;
        }
        else if (Locked == FalsePassword)
        {
            ClassName = "locked-falsePassword";
            //TODO:UnlockConfirmMessage = CoreLang.FalsePasswordUnlockConfirm;
        }
    }
}