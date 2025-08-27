using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateMasterLiteModeSettingCommand : INonQueryCommand
{
    public ValidateMasterLiteModeSettingCommand(string settingXml, string site, int sRecommend,
        string language, decimal credit, int creator, string ip, string username,
        int settingStatus, byte systemId)
    {
        Settingxml = settingXml;
        Site = site;
        Srecommend = sRecommend;
        Language = language;
        Credit = credit;
        Creator = creator;
        Ip = ip;
        Username = username;
        Settingstatus = settingStatus;
        Systemid = systemId;
    }

    public string Settingxml { get; set; }
    public string Site { get; set; }
    public int Srecommend { get; set; }
    public string Language { get; set; }
    public decimal Credit { get; set; }
    public int Creator { get; set; }
    public string Ip { get; set; }
    public string Username { get; set; }
    public int Settingstatus { get; set; }
    public byte Systemid { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustomerInsMember_SAg_Validate";
    public bool IsValid() => true;
}
