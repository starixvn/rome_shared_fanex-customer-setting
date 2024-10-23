namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType155;

public class GetWinlossLimitCriteria : GetCustomerForUpdateCriteria
{
    public GetWinlossLimitCriteria(int custId, byte settingStatus, int roleId, int groupId = 0, int groupType = 0)
        : base(custId, settingStatus, roleId, groupId, groupType)
    {
    }

    public override string GetSettingKey() => "Acc_CustSetting_WinLossLimit_155_Select";
}
