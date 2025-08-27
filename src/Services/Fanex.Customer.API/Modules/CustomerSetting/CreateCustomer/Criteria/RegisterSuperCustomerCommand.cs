using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class RegisterSuperCustomerCommand : INonQueryCommand
{
    public RegisterSuperCustomerCommand(string username, int siteDbGroupId, string site, 
        int currency, int roleId, string language)
    {
        Username = username;
        Sitedbgroupid = siteDbGroupId;
        Site = site;
        Currency = currency;
        Roleid = roleId;
        Language = language;
    }

    public string Username { get; set; }
    public int Sitedbgroupid { get; set; }
    public string Site { get; set; }
    public int Currency { get; set; }
    public int Roleid { get; set; }
    public string Language { get; set; }

    // Output parameters
    public int Msgtype { get; set; }
    public string ErrMsg { get; set; }
    public int ErrCode { get; set; }
    public int CustId { get; set; }

    public string GetSettingKey() => "Acc_CustomerIns_Register";
    public bool IsValid() => true;
}
