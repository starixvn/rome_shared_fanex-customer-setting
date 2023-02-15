namespace Alpha.CloneAccount.Criteria;

public class CopyAgentCommand : CopyCustomerCommand
{
    public CopyAgentCommand(
        string userName,
        string userpwd,
        string firstName,
        string lastName,
        string phone,
        string mobilePhone,
        string fax,
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
        : base(userName, userpwd, firstName, lastName, phone, mobilePhone, fax, credit, creator, sourceCustId, subAccountId, language, ip, systemId, settingStatus)
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

