using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateMemberSettingCommand : INonQueryCommand
{
    public ValidateMemberSettingCommand(int creator, string username, decimal credit, string settingxml,
        string language, int recommend, string ugroup, byte systemid, string ip, decimal accumcreditallcashmember)
    {
        Creator = creator;
        Username = username;
        Credit = credit;
        Settingxml = settingxml;
        Language = language;
        Recommend = recommend;
        Ugroup = ugroup;
        Systemid = systemid;
        Ip = ip;
        Accumcreditallcashmember = accumcreditallcashmember;
    }

    public int Creator { get; set; }
    public string Username { get; set; }
    public decimal Credit { get; set; }
    public string Settingxml { get; set; }
    public string Language { get; set; }
    public int Recommend { get; set; }
    public string Ugroup { get; set; }
    public byte Systemid { get; set; }
    public string Ip { get; set; }
    public decimal Accumcreditallcashmember { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_Ag_Validate";
    public bool IsValid() => true;
}
