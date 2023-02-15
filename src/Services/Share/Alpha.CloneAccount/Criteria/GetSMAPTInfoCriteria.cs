using Alpha.CloneAccount.Models;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Alpha.CloneAccount.Criteria;

public class GetSMAPTInfoCriteria : CriteriaBase
{
    public GetSMAPTInfoCriteria(UserLevel custLevel, int custId)
    {
        CustLevel = custLevel;
        CustId = custId;
    }

    [SpParam(Ignored = true)]
    public UserLevel CustLevel { get; }

    public int CustId { get; }

    public override string GetSettingKey()
        => CustLevel.IsAgentLevel()
        ? "Acc_CustomerSelAgentInfo"
        : CustLevel.IsMasterLevel()
        ? "Acc_CustomerSelMAgentInfo"
        : "Acc_CustomerSelSAgentInfo";

    public override bool IsValid()
        => CustId > 0;
}

