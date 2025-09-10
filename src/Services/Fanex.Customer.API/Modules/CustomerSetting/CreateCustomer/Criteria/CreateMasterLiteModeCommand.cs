using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class CreateMasterLiteModeCommand : INonQueryCommand
{
    public CreateMasterLiteModeCommand(int custId, string firstName, string lastName, int creator,
        string username, string userPwd, string phone, string mobilePhone, string fax, string remark,
        string transferOption, bool disableAutoMpt, string settingXml, string templateName,
        string settingTypeAll, string language, int subAccId, decimal credit, int sRecommend,
        byte systemId, string ip, string site, int settingStatus)
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
        Remark = remark;
        Transferoption = transferOption;
        Disableautompt = disableAutoMpt;
        Settingxml = settingXml;
        Templatename = templateName;
        Settingtypeall = settingTypeAll;
        Language = language;
        Subaccid = subAccId;
        Credit = credit;
        Srecommend = sRecommend;
        Systemid = systemId;
        Ip = ip;
        Site = site;
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
    public string Remark { get; set; }
    public string Transferoption { get; set; }
    public bool Disableautompt { get; set; }
    public string Settingxml { get; set; }
    public string Templatename { get; set; }
    public string Settingtypeall { get; set; }
    public string Language { get; set; }
    public int Subaccid { get; set; }
    public decimal Credit { get; set; }
    public int Srecommend { get; set; }
    public byte Systemid { get; set; }
    public string Ip { get; set; }
    public string Site { get; set; }
    public int Settingstatus { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustomerInsMember_SAg";
    public bool IsValid() => true;
}
