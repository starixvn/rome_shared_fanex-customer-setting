using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria.Cash;

public class ValidateCashMemberSetting : INonQueryCommand
{
    public ValidateCashMemberSetting(int creator, string username, string settingXml, string language, int recommend, int systemId, string ip, int settingStatus)
    {
        Creator = creator;
        Username = username;
        SettingXml = settingXml;
        Language = language;
        Recommend = recommend;
        SystemId = systemId;
        Ip = ip;
        SettingStatus = settingStatus;
    }

    public int Creator { get; }
    public string Username { get; }
    public string SettingXml { get; }
    public string Language { get; }
    public int Recommend { get; }
    public int SystemId { get; }
    public string Ip { get; }
    public int SettingStatus { get; }

    public int ErrCode { get; set; }
    public int MsgType { get; set; }
    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsMember_CashMember_Validate";
    public bool IsValid() => true;
}
