using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class CheckExistDirectMemberUsernameCommand : NonQueryCommand
{
    public CheckExistDirectMemberUsernameCommand(int uplineCustId, string userName)
    {
        UplineCustId = uplineCustId;
        UserName = userName;
    }

    public string UserName { get; }

    public int UplineCustId { get; }

    public override string GetSettingKey() => "Age_CustomerCheckExist_DirectMember";

    public override bool IsValid() => UplineCustId > 0 && !string.IsNullOrWhiteSpace(UserName);
}