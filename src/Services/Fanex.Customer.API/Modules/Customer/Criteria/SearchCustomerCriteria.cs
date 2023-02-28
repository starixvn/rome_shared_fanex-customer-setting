using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class SearchCustomerCriteria : CriteriaBase
{
    public SearchCustomerCriteria(
        int userId,
        int custId,
        string userName,
        int isSubAccount,
        int excludeMember,
        int directDownLine,
        int isAccInfo,
        bool isInternalAgent)
    {
        UserId = userId;
        CustId = custId;
        UserName = userName;
        IsSubAccount = isSubAccount;
        ExcludeMember = excludeMember;
        DirectDownLine = directDownLine;
        IsAccInfo = isAccInfo;
        IsInternalAgent = isInternalAgent;
    }

    public int UserId { get; }

    public int CustId { get; }

    public string UserName { get; }

    public int IsSubAccount { get; }

    public int ExcludeMember { get; }

    public int DirectDownLine { get; }

    public int IsAccInfo { get; }

    public bool IsInternalAgent { get; }

    public override string GetSettingKey() => "Age_CustomerSelUserSearch";

    public override bool IsValid() => CustId > 0;
}