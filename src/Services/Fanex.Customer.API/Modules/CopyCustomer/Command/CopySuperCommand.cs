using Fanex.Customer.API.Shared.Constants;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class CopySuperCommand : CopyCustomerCommand
{
    public CopySuperCommand(
        string site,
        bool isInternal,
        string userName,
        string userpwd,
        decimal credit,
        int creator,
        int sourceCustId,
        string language,
        string ip,
        byte systemId,
        byte settingStatus)
        : base(userName, userpwd, credit, creator, sourceCustId, language, ip, systemId, settingStatus)
    {
        Site = site;
        IsInternal = isInternal;
    }

    public string Site { get; set; }

    public bool IsInternal { get; set; }

    // [SpParam(Ignored = true)]
    // public int MsgType { get; set; }

    public override string GetSettingKey() => "Acc_CustomerInsMember_CloneSetting_Com";

    public override bool IsValid()
        => base.IsValid()
        && !string.IsNullOrWhiteSpace(Site)
        && (SystemId == Systems.NAP || SystemId == Systems.EXT);
}
