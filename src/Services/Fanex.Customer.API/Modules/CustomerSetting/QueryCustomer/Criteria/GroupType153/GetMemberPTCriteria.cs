namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType153;

public class GetMemberPTCriteria : GetAgentPTCriteria
{
    public GetMemberPTCriteria(int custId, int groupId, int roleId, byte settingStatus)
        : base(custId, groupId, settingStatus)
    {
        RoleId = roleId;
    }

    public int RoleId { get; }

    public override string GetSettingKey()
        => "Acc_CustSetting_MemberPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
