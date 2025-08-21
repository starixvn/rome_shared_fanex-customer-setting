using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class CreateMemberLiteModeCommand : INonQueryCommand
{
    public CreateMemberLiteModeCommand(int custId, string firstName, string lastName, int creator,
        string username, string userPwd, string phone, string mobilePhone, string fax,
        string settingXml, string templateName, string settingTypeAll, string language,
        int subAccId, decimal credit, int recommend, string uGroup, byte systemId, string ip,
        int settingStatus)
    {
        Custid = custId;
        Firstname = firstName;
        Lastname = lastName;
        Creator = creator;
        Username = username;
        Userpwd = userPwd;
        Phone = phone;
        Mobilephone = mobilePhone;
        Fax = fax;
        Settingxml = settingXml;
        Templatename = templateName;
        Settingtypeall = settingTypeAll;
        Language = language;
        Subaccid = subAccId;
        Credit = credit;
        Recommend = recommend;
        Ugroup = uGroup;
        Systemid = systemId;
        Ip = ip;
        Settingstatus = settingStatus;
    }

    public int Custid { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public int Creator { get; set; }
    public string Username { get; set; }
    public string Userpwd { get; set; }
    public string Phone { get; set; }
    public string Mobilephone { get; set; }
    public string Fax { get; set; }
    public string Settingxml { get; set; }
    public string Templatename { get; set; }
    public string Settingtypeall { get; set; }
    public string Language { get; set; }
    public int Subaccid { get; set; }
    public decimal Credit { get; set; }
    public int Recommend { get; set; }
    public string Ugroup { get; set; }
    public byte Systemid { get; set; }
    public string Ip { get; set; }
    public int Settingstatus { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustomerInsMember_Ag";
    public bool IsValid() => true;
}
