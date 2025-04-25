using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.RegisterCashMember.Criteria;

public class RegisterCashMemberCommand : INonQueryCommand
{
    public int AgentId { get; }
    public string UserName { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string PhoneNumber { get; }
    public int ErrCode { get; set; }
    public string ErrMsg { get; set; } = string.Empty;

    public RegisterCashMemberCommand(int agentId, string userName, string firstName, string lastName, string phoneNumber)
    {
        AgentId = agentId;
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public bool IsValid()
        => !string.IsNullOrEmpty(UserName)
            && !string.IsNullOrEmpty(FirstName)
            && !string.IsNullOrEmpty(LastName)
            && !string.IsNullOrEmpty(PhoneNumber);

    public string GetSettingKey() => "RegisterCashMember";
}