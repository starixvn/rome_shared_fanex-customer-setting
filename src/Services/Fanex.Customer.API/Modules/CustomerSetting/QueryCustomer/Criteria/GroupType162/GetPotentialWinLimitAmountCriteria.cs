using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType162;

public class GetPotentialWinLimitAmountCriteria(int custId) : CriteriaBase
{
    public int CustId { get; } = custId;

    public override bool IsValid() => CustId > 0;

    public override string GetSettingKey() => "Acc_CustSetting_AccumWinLoss_162_Select";
}
