using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Keno.Criteria;

public class AgentGetMemberSettingCriteria : CriteriaBase
{
    public AgentGetMemberSettingCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey() => "Acc_CustomerSelEditMemberKeno";

    public override bool IsValid() => CustId > 0;
}