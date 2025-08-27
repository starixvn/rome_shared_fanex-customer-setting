using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class ValidateSuperSettingCommand : INonQueryCommand
{
    public ValidateSuperSettingCommand(int currency, string site, int creator, string username, 
        string language, decimal credit, int settingStatus, string settingXml)
    {
        Currency = currency;
        Site = site;
        Creator = creator;
        Username = username;
        Language = language;
        Credit = credit;
        Settingstatus = settingStatus;
        Settingxml = settingXml;
    }

    public int Currency { get; set; }
    public string Site { get; set; }
    public int Creator { get; set; }
    public string Username { get; set; }
    public string Language { get; set; }
    public decimal Credit { get; set; }
    public int Settingstatus { get; set; }
    public string Settingxml { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_Com_Validate";
    public bool IsValid() => true;
}
