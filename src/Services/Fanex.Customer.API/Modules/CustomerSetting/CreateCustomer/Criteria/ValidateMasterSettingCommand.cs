using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateMasterSettingCommand : INonQueryCommand
{
    public ValidateMasterSettingCommand(string site, int creator, string username, int srecommend,
        string language, decimal credit, string ip, int settingstatus, string settingxml, byte systemid)
    {
        Site = site;
        Creator = creator;
        Username = username;
        Srecommend = srecommend;
        Language = language;
        Credit = credit;
        Ip = ip;
        Settingstatus = settingstatus;
        Settingxml = settingxml;
        Systemid = systemid;
    }

    public string Site { get; set; }
    public int Creator { get; set; }
    public string Username { get; set; }
    public int Srecommend { get; set; }
    public string Language { get; set; }
    public decimal Credit { get; set; }
    public string Ip { get; set; }
    public int Settingstatus { get; set; }
    public string Settingxml { get; set; }
    public byte Systemid { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_SAg_Validate";
    public bool IsValid() => true;
}
