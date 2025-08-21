using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateDirectMemberSettingCommand : INonQueryCommand
{
    public ValidateDirectMemberSettingCommand(int creator, string username, decimal credit, string settingxml,
        string language, int uplineid, string ugroup, byte systemid, string ip)
    {
        Creator = creator;
        Username = username;
        Credit = credit;
        Settingxml = settingxml;
        Language = language;
        Uplineid = uplineid;
        Ugroup = ugroup;
        Systemid = systemid;
        Ip = ip;
    }

    public int Creator { get; set; }
    public string Username { get; set; }
    public decimal Credit { get; set; }
    public string Settingxml { get; set; }
    public string Language { get; set; }
    public int Uplineid { get; set; }
    public string Ugroup { get; set; }
    public byte Systemid { get; set; }
    public string Ip { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_DirectMember_Validate";
    public bool IsValid() => true;
}
