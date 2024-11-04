using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class UsernameSuggestionByRoleCriteria : CriteriaBase
{
    public UsernameSuggestionByRoleCriteria(
        int custId,
        string username,
        byte roleId
        )
    {
        CustId = custId;
        Username = username;
        RoleId = roleId;
    }

    public int CustId { get; }

    public string Username { get; }

    public byte RoleId { get; }

    public override string GetSettingKey() => "Age_CustomerSelUserSearch_ByRole";

    public override bool IsValid() => CustId > 0 && !string.IsNullOrEmpty(Username);
}