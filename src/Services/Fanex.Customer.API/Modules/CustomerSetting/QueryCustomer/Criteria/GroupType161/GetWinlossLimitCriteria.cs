namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType155;

public class GetWinLimitCriteria : GetCustomerForUpdateCriteria
{
    public GetWinLimitCriteria(int custId, byte settingStatus, int roleId, int groupId = 0, int groupType = 0)
        : base(custId, settingStatus, roleId, groupId, groupType)
    {
    }

    public override string GetSettingKey() => "Acc_CustSetting_WinLimit_161_Select";
}
