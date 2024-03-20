namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;

public class GetMasterPTCriteria : GetSuperPTCriteria
{
    public GetMasterPTCriteria(int custId, int groupId, byte settingStatus)
        : base(custId, groupId, settingStatus)
    {
    }

    public override string GetSettingKey()
        => "Acc_CustSetting_MasterPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
