using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

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
        : base(userName, userpwd, credit, creator, sourceCustId, language, ip, systemId, settingStatus)
    {
        MRecommend = mrecommend;
        MaxCredit = maxCredit;
        SubAccId = subAccountId;
    }

    public int MRecommend { get; }

    public decimal MaxCredit { get; }

    public int SubAccId { get; }

    // [SpParam(Ignored = true)]
    // public int MsgType { get; set; }

    public override string GetSettingKey()
        => "Acc_CustomerInsMember_CloneSetting_MAg";

    public override bool IsValid()
        => base.IsValid() && MRecommend > 0;
}

