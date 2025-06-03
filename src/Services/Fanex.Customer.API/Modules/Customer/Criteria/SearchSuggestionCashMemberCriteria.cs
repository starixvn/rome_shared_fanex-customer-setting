using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class SearchSuggestionCashMemberCriteria : CriteriaBase
{
    public SearchSuggestionCashMemberCriteria(
        int uplineCustId,
        string searchUserName)
    {
        UplineCustId = uplineCustId;
        UserName = searchUserName;
    }

    public int UplineCustId { get; }

    public string UserName { get; }

    public int Mode => 1; // Cash member only

    public override string GetSettingKey() => "Acc_CustomerInsMember_Suggestion";

    public override bool IsValid() => UplineCustId > 0 && !string.IsNullOrWhiteSpace(UserName);
}