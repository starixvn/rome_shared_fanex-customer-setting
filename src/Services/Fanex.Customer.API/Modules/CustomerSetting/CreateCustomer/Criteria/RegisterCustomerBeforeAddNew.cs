using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class RegisterCustomerBeforeAddNew : INonQueryCommand
{
    public RegisterCustomerBeforeAddNew(string username, int siteDbGroupId, string site, byte roleId, int uplineId, string language)
    {
        Username = username;
        SiteDbGroupId = siteDbGroupId;
        Site = site;
        RoleId = roleId;
        UplineId = uplineId;
        Language = language;
    }

    public string Username { get; }
    public int SiteDbGroupId { get; }
    public string Site { get; }
    public byte RoleId { get; }
    public int UplineId { get; }
    public string Language { get; }

    // Output properties
    public int CustId { get; set; }
    public int ErrCode { get; set; }
    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Acc_CustomerIns_Register";
    public bool IsValid() => true;
}
