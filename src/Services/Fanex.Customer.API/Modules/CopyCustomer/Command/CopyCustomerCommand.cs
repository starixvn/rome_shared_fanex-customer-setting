using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public abstract class CopyCustomerCommand : NonQueryCommand
{
    protected CopyCustomerCommand(
        string userName,
        string userpwd,
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

    public string FirstName { get; set; } = string.Empty;


    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Fax { get; set; } = string.Empty;

    public string MobilePhone { get; set; } = string.Empty;

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

