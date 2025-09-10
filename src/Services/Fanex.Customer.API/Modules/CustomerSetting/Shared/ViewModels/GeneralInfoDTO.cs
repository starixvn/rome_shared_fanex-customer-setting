namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class GeneralInformationDTO
{
    public int CustId { get; set; }

    public string UserName { get; set; }

    public byte RoleId { get; set; }

    public byte UplineRoleId { get; set; }

    public int UplineCustId { get; set; }

    public string UplineUserName { get; set; }

    public string NewUserName { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public string MobilePhone { get; set; }

    public decimal MaxCredit { get; set; }

    public decimal MaxCreditMin { get; set; }

    public decimal MaxCreditMax { get; set; }

    public decimal MemberMaxCredit { get; set; }

    public decimal MemberMaxCreditMin { get; set; }

    public string Fax { get; set; }

    public string Remark { get; set; }

    public bool DisableAutoMPT { get; set; }

    public string Group { get; set; }

    public string TransferOption { get; set; }

    public bool IsAutomateTransfer { get; set; }

    public string Site { get; set; }

    public string SiteLabel { get; set; }

    public int Currency { get; set; }

    public string TemplateName { get; set; }

    public bool IsInternal { get; set; }
    
    public int BankId { get; set; }

    public string AccountNumber { get; set; }
}