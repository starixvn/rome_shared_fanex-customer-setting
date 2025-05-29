using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType162;

public class GetPotentialWinLimitCriteria(int custId, byte settingStatus) : CriteriaBase
{
    public int CustId { get; } = custId;

    public byte SettingStatus { get; } = settingStatus;

    public override bool IsValid() => CustId > 0;

    public override string GetSettingKey() => "Acc_CustSetting_PotentialWinLimit_162_Select";
}
