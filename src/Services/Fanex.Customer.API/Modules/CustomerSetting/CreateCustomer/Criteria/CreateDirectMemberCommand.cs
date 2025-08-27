using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class CreateDirectMemberCommand : INonQueryCommand
{
    public CreateDirectMemberCommand(int custid, string firstname, string lastname, int creator, string username,
        string userpwd, string phone, string mobilephone, string fax, string remark, decimal credit, string settingxml,
        string language, int subaccid, int uplineid, string ugroup, byte systemid, string ip, int settingstatus)
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
        Remark = remark;
        Credit = credit;
        Settingxml = settingxml;
        Language = language;
        Subaccid = subaccid;
        Uplineid = uplineid;
        Ugroup = ugroup;
        Systemid = systemid;
        Ip = ip;
        Settingstatus = settingstatus;
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
    public decimal Credit { get; set; }
    public string Settingxml { get; set; }
    public string Language { get; set; }
    public int Subaccid { get; set; }
    public int Uplineid { get; set; }
    public string Ugroup { get; set; }
    public byte Systemid { get; set; }
    public string Ip { get; set; }
    public int Settingstatus { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_DirectMember";
    public bool IsValid() => true;
}
