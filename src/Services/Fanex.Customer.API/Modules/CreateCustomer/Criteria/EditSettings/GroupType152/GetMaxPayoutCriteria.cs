namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType152;

public class GetMaxPayoutCriteria : GetCustomerForUpdateCriteria
{
    public GetMaxPayoutCriteria(int custId, byte settingStatus, int roleId, int groupId = 0, int groupType = 0)
        : base(custId, settingStatus, roleId, groupId, groupType)
    {
    }

    public override string GetSettingKey() => "Acc_CustSetting_MaxPayoutSelect";
}
