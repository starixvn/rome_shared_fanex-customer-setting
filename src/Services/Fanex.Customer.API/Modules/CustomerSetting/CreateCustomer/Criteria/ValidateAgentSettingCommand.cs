using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateAgentSettingCommand : INonQueryCommand
{
    public ValidateAgentSettingCommand(string language, int mrecommend, decimal credit, decimal maxcredit,
        int creator, string username, string ip, int settingstatus, string settingxml, byte systemid, string site)
    {
        Language = language;
        Mrecommend = mrecommend;
        Credit = credit;
        Maxcredit = maxcredit;
        Creator = creator;
        Username = username;
        Ip = ip;
        Settingstatus = settingstatus;
        Settingxml = settingxml;
        Systemid = systemid;
        Site = site;
    }

    public string Language { get; set; }
    public int Mrecommend { get; set; }
    public decimal Credit { get; set; }
    public decimal Maxcredit { get; set; }
    public int Creator { get; set; }
    public string Username { get; set; }
    public string Ip { get; set; }
    public int Settingstatus { get; set; }
    public string Settingxml { get; set; }
    public byte Systemid { get; set; }
    public string Site { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_MAg_Validate";
    public bool IsValid() => true;
}
