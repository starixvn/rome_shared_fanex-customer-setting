using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class GetCashAgentBanksCriteria : CriteriaBase
{
    public GetCashAgentBanksCriteria(int agentId, int transType)
    {
        AgentID = agentId;
        TransType = transType;
        IsManual = null;
    }

    public int AgentID { get; }

    public int TransType { get; }

    [SpParam(AllowNull = true)]
    public int? IsManual { get; set; }

    public override string GetSettingKey() => "Age_CashAgentBankInfoSel";

    public override bool IsValid() => AgentID > 0;
}