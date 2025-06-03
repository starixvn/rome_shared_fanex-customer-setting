using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class CheckExistCashMemberUsernameCommand : NonQueryCommand
{
    public CheckExistCashMemberUsernameCommand(int uplineCustId, string userName)
    {
        UplineCustId = uplineCustId;
        UserName = userName;
    }

    public int UplineCustId { get; }

    public string UserName { get; }

    public int Mode => 1; // Cash member only

    public override string GetSettingKey() => "Acc_CustomerInsMember_CheckExists";

    public override bool IsValid() => UplineCustId > 0 && !string.IsNullOrWhiteSpace(UserName);
}