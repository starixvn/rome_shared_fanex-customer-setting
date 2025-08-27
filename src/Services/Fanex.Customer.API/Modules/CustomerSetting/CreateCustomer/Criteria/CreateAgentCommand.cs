using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class CreateAgentCommand : INonQueryCommand
{
    public CreateAgentCommand(int custid, string firstname, string lastname, int creator, string username,
        string userpwd, string phone, string mobilephone, string fax, string transferoption, string language,
        int subaccid, decimal credit, decimal maxcredit, int mrecommend, byte systemid, string ip,
        string site, int settingstatus, string settingxml)
    {
        Custid = custid;
        Firstname = firstname;
        Lastname = lastname;
        Creator = creator;
        Username = username;
        Userpwd = userpwd;
        Phone = phone;
        Mobilephone = mobilephone;
        Fax = fax;
        Transferoption = transferoption;
        Language = language;
        Subaccid = subaccid;
        Credit = credit;
        Maxcredit = maxcredit;
        Mrecommend = mrecommend;
        Systemid = systemid;
        Ip = ip;
        Site = site;
        Settingstatus = settingstatus;
        Settingxml = settingxml;
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
    public string Transferoption { get; set; }
    public string Language { get; set; }
    public int Subaccid { get; set; }
    public decimal Credit { get; set; }
    public decimal Maxcredit { get; set; }
    public int Mrecommend { get; set; }
    public byte Systemid { get; set; }
    public string Ip { get; set; }
    public string Site { get; set; }
    public int Settingstatus { get; set; }
    public string Settingxml { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_MAg";
    public bool IsValid() => true;
}
