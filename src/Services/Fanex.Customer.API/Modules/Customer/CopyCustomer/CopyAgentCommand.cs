namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class CopyAgentCommand : CopyCustomerCommand
{
    public CopyAgentCommand(
        string userName,
        string userpwd,
        decimal credit,
        decimal maxCredit,
        int creator,
        int mrecommend,
        int sourceCustId,
        int subAccountId,
        string language,
        string ip,
        byte systemId,
        byte settingStatus)
        : base(userName, userpwd, credit, creator, sourceCustId, subAccountId, language, ip, systemId, settingStatus)
    {
        MRecommend = mrecommend;
        MaxCredit = maxCredit;
    }

    public int MRecommend { get; }

    public decimal MaxCredit { get; }

    public override string GetSettingKey()
        => "Acc_CustomerInsMember_CloneSetting_MAg";

    public override bool IsValid()
        => base.IsValid() && MRecommend > 0;
}

