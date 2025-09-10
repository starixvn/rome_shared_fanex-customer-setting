using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateAgentLiteModeSettingCommand : INonQueryCommand
{
    public ValidateAgentLiteModeSettingCommand(int creator, string username, string settingXml,
        string language, decimal credit, decimal maxCredit, int mRecommend, string ip,
        byte systemId, int settingStatus)
    {
        Creator = creator;
        Username = username;
        Settingxml = settingXml;
        Language = language;
        Credit = credit;
        Maxcredit = maxCredit;
        Mrecommend = mRecommend;
        Ip = ip;
        Systemid = systemId;
        Settingstatus = settingStatus;
    }

    public int Creator { get; set; }
    public string Username { get; set; }
    public string Settingxml { get; set; }
    public string Language { get; set; }
    public decimal Credit { get; set; }
    public decimal Maxcredit { get; set; }
    public int Mrecommend { get; set; }
    public string Ip { get; set; }
    public byte Systemid { get; set; }
    public int Settingstatus { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustomerInsMember_MAg_Validate";
    public bool IsValid() => true;
}
