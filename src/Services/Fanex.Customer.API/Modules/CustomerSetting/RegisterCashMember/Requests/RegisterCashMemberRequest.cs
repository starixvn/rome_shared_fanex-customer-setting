namespace Fanex.Customer.API.Modules.CustomerSetting.RegisterCashMember.Requests;

public class RegisterCashMemberRequest
{
    public required int AgentId { get; set; }
    public required string UserName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Language { get; set; }
}
