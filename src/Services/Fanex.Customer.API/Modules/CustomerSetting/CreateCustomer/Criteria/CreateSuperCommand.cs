using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class CreateSuperCommand : INonQueryCommand
{
    public CreateSuperCommand(int custId, string firstName, string lastName, int creator, string username,
        string userPwd, string phone, string mobilePhone, string fax, string language, decimal credit,
        byte systemId, string ip, int settingStatus, string site, int currency, bool isInternal, string settingXml)
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
        Language = language;
        Credit = credit;
        Systemid = systemId;
        Ip = ip;
        Settingstatus = settingStatus;
        Site = site;
        Currency = currency;
        Isinternal = isInternal;
        Settingxml = settingXml;
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
    public string Language { get; set; }
    public decimal Credit { get; set; }
    public byte Systemid { get; set; }
    public string Ip { get; set; }
    public int Settingstatus { get; set; }
    public string Site { get; set; }
    public int Currency { get; set; }
    public bool Isinternal { get; set; }
    public string Settingxml { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_Com";
    public bool IsValid() => true;
}
