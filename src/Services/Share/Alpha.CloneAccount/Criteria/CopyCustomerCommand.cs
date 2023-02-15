using Alpha.CloneAccount.Configurations;
using Fanex.Data.Repository;

namespace Alpha.CloneAccount.Criteria;

public abstract class CopyCustomerCommand : NonQueryCommand
{
    protected CopyCustomerCommand(
        string userName,
        string userpwd,
        string firstName,
        string lastName,
        string phone,
        string mobilePhone,
        string fax,
        decimal credit,
        int creator,
        int sourceCustId,
        int subAccountId,
        string language,
        string ip,
        byte systemId,
        byte settingStatus)
    {
        UserName = userName;
        UserPwd = userpwd;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        MobilePhone = mobilePhone;
        Fax = fax;
        Credit = credit;
        Creator = creator;
        ExistingCustId = sourceCustId;
        SubAccountId = subAccountId;
        Language = language;
        IP = ip;
        SystemId = systemId;
        SettingStatus = settingStatus;
    }

    public string UserName { get; }

    public string UserPwd { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string Phone { get; }

    public string MobilePhone { get; }

    public string Fax { get; }

    public decimal Credit { get; }

    public int Creator { get; }

    public int ExistingCustId { get; }

    public int SubAccountId { get; }

    public string Language { get; }

    public string IP { get; }

    public byte SystemId { get; }

    public int CustId { get; set; }

    public byte SettingStatus { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override bool IsValid()
        => !string.IsNullOrWhiteSpace(UserName)
        && !string.IsNullOrWhiteSpace(UserPwd)
        && Credit >= 0
        && Creator > 0
        && ExistingCustId > 0
        && SystemId > 0;
}

