namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;

public class GetMasterPTCriteria : GetSuperPTCriteria
{
    // public GetMasterPTCriteria(int custId, byte settingStatus) : base(custId, settingStatus)
    // {
    // }

    public override string GetSettingKey()
        => "Acc_CustSetting_MasterPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
