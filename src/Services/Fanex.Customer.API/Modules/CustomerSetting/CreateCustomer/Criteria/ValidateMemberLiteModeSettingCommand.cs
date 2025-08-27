using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateMemberLiteModeSettingCommand : INonQueryCommand
{
    public ValidateMemberLiteModeSettingCommand(int creator, string username, string settingXml,
        string language, decimal credit, int recommend, string uGroup, string ip,
        byte systemId, int settingStatus, decimal accumCreditAllCashMember)
    {
        Creator = creator;
        Username = username;
        Settingxml = settingXml;
        Language = language;
        Credit = credit;
        Recommend = recommend;
        Ugroup = uGroup;
        Ip = ip;
        Systemid = systemId;
        Settingstatus = settingStatus;
        Accumcreditallcashmember = accumCreditAllCashMember;
    }

    public int Creator { get; set; }
    public string Username { get; set; }
    public string Settingxml { get; set; }
    public string Language { get; set; }
    public decimal Credit { get; set; }
    public int Recommend { get; set; }
    public string Ugroup { get; set; }
    public string Ip { get; set; }
    public byte Systemid { get; set; }
    public int Settingstatus { get; set; }
    public decimal Accumcreditallcashmember { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustomerInsMember_Ag_Validate";
    public bool IsValid() => true;
}
