namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType154;

public class GetBetSettingWithMaxPerBallCriteria : GetCustomerForUpdateCriteria
{
    public GetBetSettingWithMaxPerBallCriteria(int custId, byte settingStatus, int roleId, int groupId = 0, int groupType = 0)
        : base(custId, settingStatus, roleId, groupId, groupType)
    {
    }

    public override string GetSettingKey() => "Acc_CustSetting_BetSetting_154_Select";
}
