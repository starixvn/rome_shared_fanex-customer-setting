namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType151;

public class GetBetSettingCriteria : GetCustomerForUpdateCriteria
{
    public GetBetSettingCriteria(int custId, byte settingStatus, int roleId, int groupId = 0, int groupType = 0)
        : base(custId, settingStatus, roleId, groupId, groupType)
    {
    }

    public override string GetSettingKey() => "Acc_CustSetting_BetSetting151Sel";
}
