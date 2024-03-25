namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;

public class GetAgentPTCriteria : GetSuperPTCriteria
{
    // public GetAgentPTCriteria(int custId, byte settingStatus) : base(custId, settingStatus)
    // {
    // }

    public bool DisableAutoMPT { get; set; }

    public override string GetSettingKey()
        => "Acc_CustSetting_AgentPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
