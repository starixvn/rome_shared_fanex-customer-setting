using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class GetCashAgentBanksCriteria : CriteriaBase
{
    public GetCashAgentBanksCriteria(int agentId, int transType)
    {
        AgentID = agentId;
        TransType = transType;
    }

    public int AgentID { get; }

    public int TransType { get; }

    public override string GetSettingKey() => "Age_CashAgentBankInfoSel";

    public override bool IsValid() => AgentID > 0;
}