namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;

public class GetMemberPTCriteria : GetAgentPTCriteria
{
    // public GetMemberPTCriteria(int custId, byte settingStatus, int roleId) : base(custId, settingStatus)
    // {
    //     RoleId = roleId;
    // }

    public int RoleId { get; set; }

    public override string GetSettingKey()
        => "Acc_CustSetting_MemberPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
