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
        : base(userName, userpwd, credit, creator, sourceCustId, subAccountId, language, ip, systemId, settingStatus)
    {
        SRecommend = srecommend;
    }

    public int SRecommend { get; }

    public override string GetSettingKey()
        => "Acc_CustomerInsMember_CloneSetting_SAg";

    public override bool IsValid()
        => base.IsValid() && SRecommend > 0;
}

