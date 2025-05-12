using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCurrentPotentialWinLimitAmountCriteria(int custId) : CriteriaBase
{
    public int CustId { get; } = custId;

    public override string GetSettingKey() => $"Acc_CustSetting_AccumWinLoss_162_Select";

    public override bool IsValid() => CustId > 0;
}
