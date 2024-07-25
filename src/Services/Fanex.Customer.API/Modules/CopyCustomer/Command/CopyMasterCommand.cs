namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class CopyMasterCommand : CopyCustomerCommand
{
    public CopyMasterCommand(
        string userName,
        string userpwd,
        decimal credit,
        int creator,
        int srecommend,
        int sourceCustId,
        int subAccountId,
        string language,
        string ip,
        byte systemId,
        byte settingStatus)
        : base(userName, userpwd, credit, creator, sourceCustId, language, ip, systemId, settingStatus)
    {
        SRecommend = srecommend;
        SubAccId = subAccountId;
    }

    public int SRecommend { get; }

    public int SubAccId { get; }

    // [SpParam(Ignored = true)]
    // public int MsgType { get; set; }

    public override string GetSettingKey()
        => "Acc_CustomerInsMember_CloneSetting_SAg";

    public override bool IsValid()
        => base.IsValid() && SRecommend > 0;
}

