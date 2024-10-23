namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType153;

public class GetAgentPTCriteria : GetSuperPTCriteria
{
    public GetAgentPTCriteria(int custId, int groupId, byte settingStatus)
        : base(custId, groupId, settingStatus)
    {
    }

    public bool DisableAutoMPT { get; set; }

    public override string GetSettingKey()
        => "Acc_CustSetting_AgentPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
