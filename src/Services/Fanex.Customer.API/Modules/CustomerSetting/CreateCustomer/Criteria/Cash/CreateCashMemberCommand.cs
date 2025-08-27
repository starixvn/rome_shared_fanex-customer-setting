using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria.Cash;

public class CreateCashMemberCommand : INonQueryCommand
{
    public CreateCashMemberCommand(
        int custId,
        string firstName,
        string lastName,
        int creator,
        string username,
        string userPwd,
        string phone,
        string mobilePhone,
        string remark,
        int bankId,
        string accountNumber,
        string settingXml,
        string language,
        int subAccId,
        int uplineId,
        int systemId,
        string ip,
        int settingStatus)
    {
        CustId = custId;
        FirstName = firstName;
        LastName = lastName;
        Creator = creator;
        Username = username;
        UserPwd = userPwd;
        Phone = phone;
        MobilePhone = mobilePhone;
        Remark = remark;
        BankId = bankId;
        AccountNumber = accountNumber;
        SettingXml = settingXml;
        Language = language;
        SubAccId = subAccId;
        UplineId = uplineId;
        SystemId = systemId;
        Ip = ip;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public int Creator { get; }
    public string Username { get; }
    public string UserPwd { get; }
    public string Phone { get; }
    public string MobilePhone { get; }
    public string Remark { get; }
    public int BankId { get; }
    public string AccountNumber { get; }
    public string SettingXml { get; }
    public string Language { get; }
    public int SubAccId { get; }
    public int UplineId { get; }
    public int SystemId { get; }
    public string Ip { get; }
    public int SettingStatus { get; }

    // Output properties
    public int ErrCode { get; set; }
    public int MsgType { get; set; }
    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_CashMember";
    public bool IsValid() => true;
}
