namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels;

public class GeneralInfomation
{
    public string UserName { get; set; }

    public string UplineUserName { get; set; }

    public string NewUserName { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public string MobilePhone { get; set; }

    public decimal MaxCredit { get; set; }

    public decimal MemberMaxCredit { get; set; }

    public string Fax { get; set; }

    public bool AllowAutoPT { get; set; }

    public string Group { get; set; }

    public string TransferOption { get; set; }

    public bool IsInTransferList { get; set; }
}