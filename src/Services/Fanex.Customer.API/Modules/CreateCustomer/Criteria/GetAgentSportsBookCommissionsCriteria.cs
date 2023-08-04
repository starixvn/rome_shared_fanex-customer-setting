using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class GetAgentSportsBookCommissionsCriteria : CriteriaBase
{
    public GetAgentSportsBookCommissionsCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_AgentCommSBSel";

    public override bool IsValid() => CustId > 0;
}