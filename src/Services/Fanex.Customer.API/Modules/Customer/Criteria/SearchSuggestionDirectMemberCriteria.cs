using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class SearchSuggestionDirectMemberCriteria : CriteriaBase
{
    public SearchSuggestionDirectMemberCriteria(
        int uplineCustId,
        string searchUserName)
    {
        UplineCustId = uplineCustId;
        UserName = searchUserName;
    }

    public int UplineCustId { get; }

    public string UserName { get; }

    public override string GetSettingKey() => "Age_CustomerSearch_DirectMember";

    public override bool IsValid() => UplineCustId > 0 && !string.IsNullOrWhiteSpace(UserName);
}