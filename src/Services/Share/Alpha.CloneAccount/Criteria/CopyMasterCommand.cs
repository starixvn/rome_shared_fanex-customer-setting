namespace Alpha.CloneAccount.Criteria;

public class CopyMasterCommand : CopyCustomerCommand
{
    public CopyMasterCommand(
        string userName,
        string userpwd,
        string firstName,
        string lastName,
        string phone,
        string mobilePhone,
        string fax,
        decimal credit,
        int creator,
        int srecommend,
        int sourceCustId,
        int subAccountId,
        string language,
        string ip,
        byte systemId,
        byte settingStatus)
        : base(userName, userpwd, firstName, lastName, phone, mobilePhone, fax, credit, creator, sourceCustId, subAccountId, language, ip, systemId, settingStatus)
    {
        SRecommend = srecommend;
    }

    public int SRecommend { get; }

    public override string GetSettingKey()
        => "Acc_CustomerInsMember_CloneSetting_SAg";

    public override bool IsValid()
        => base.IsValid() && SRecommend > 0;
}

