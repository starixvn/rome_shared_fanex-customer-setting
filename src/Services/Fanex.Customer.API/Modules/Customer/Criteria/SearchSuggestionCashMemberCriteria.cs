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

    public override string GetSettingKey() => "Age_CustomerSearch_CashMember";

    public override bool IsValid() => UplineCustId > 0 && !string.IsNullOrWhiteSpace(UserName);
}