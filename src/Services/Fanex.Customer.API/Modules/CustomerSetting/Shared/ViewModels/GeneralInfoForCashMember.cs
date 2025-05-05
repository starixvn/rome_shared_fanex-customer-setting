using Fanex.Customer.API.Shared.Constants;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class GeneralInfoForCashMember
{
    public GeneralInfoForCashMember()
    { }

    public GeneralInfoForCashMember(GeneralInformationDTO dto)
    {
        CustId = dto.CustId;
        UserName = dto.UserName;
        UplineCustId = dto.UplineCustId;
        UplineUserName = dto.UplineUserName;
        NewUserName = dto.NewUserName;
        Password = dto.Password;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Phone = dto.Phone;
        MobilePhone = dto.MobilePhone;
        Fax = dto.Fax;
        Site = dto.Site;
        SiteLabel = dto.SiteLabel;
        Currency = dto.Currency;
    }

    public int CustId { get; set; }

    public string UserName { get; set; }

    public byte RoleId => CustomerLevel.Member;

    public byte UplineRoleId => CustomerLevel.Agent;

    public int UplineCustId { get; set; }

    public string UplineUserName { get; set; }

    public string NewUserName { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public string MobilePhone { get; set; }

    public string Fax { get; set; }

    public string Site { get; set; }

    public string SiteLabel { get; set; }

    public int Currency { get; set; }

    public int BankId { get; set; }
}