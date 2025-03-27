using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType150;

public class GetSBCommissionCriteria : CriteriaBase
{


    private readonly Dictionary<int, string> SPNamesMapping = new()
    {
        {1, "Acc_CustSetting_Discount150Sel_Member"},
        {2, "Acc_CustSetting_Discount150Sel_Agent"},
        {3, "Acc_CustSetting_Discount150Sel_Master"},
        {4, "Acc_CustSetting_Discount150Sel_Super"},
        {1_4, "Acc_CustSetting_Discount150Sel_SuperDirectMember"},
        {1_3, "Acc_CustSetting_Discount150Sel_MasterDirectMember"},
    };

    public GetSBCommissionCriteria(int custId, byte roleId, int uplineRoleId, int? agentId, byte settingStatus)
    {
        CustId = custId;
        RoleId = roleId;
        UplineRoleId = uplineRoleId;
        SettingStatus = settingStatus;
        AgentId = agentId ?? 0;
    }

    public int CustId { get; }

    public int UplineRoleId { get; }

    public byte SettingStatus { get; }

    [SpParam(Ignored = true)]
    public byte RoleId { get; }

    [SpParam(Ignored = true)]
    public int AgentId { get; }

    public override string GetSettingKey() => SPNamesMapping[AgentId < 0 ? RoleId * 10 + (-AgentId) : RoleId];

    public override bool IsValid() => CustId > 0;
}
