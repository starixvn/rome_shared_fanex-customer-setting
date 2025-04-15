using System.Collections;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType156;

public class GetCommissionCriteria : CriteriaBase
{
    public GetCommissionCriteria(int custId, int groupId, byte roleId, int uplineRoleId, int? agentId, byte settingStatus)
    {
        CustId = custId;
        GroupId = groupId;
        RoleId = roleId;
        UplineRoleId = uplineRoleId;
        SettingStatus = settingStatus;
        AgentId = agentId ?? 0;
    }

    public int CustId { get; }

    public int UplineRoleId { get; }

    public byte SettingStatus { get; }

    public int GroupId { get; }

    [SpParam(Ignored = true)]
    public byte RoleId { get; }

    [SpParam(Ignored = true)]
    public int AgentId { get; }

    public override string GetSettingKey() => "Acc_CustSetting_Discount_156_Select";

    public override bool IsValid() => CustId > 0;

    public Hashtable GetParameters()
        => new Hashtable
        {
            { "CustId", CustId },
            { "UplineRoleId", UplineRoleId },
            { "SettingStatus", SettingStatus },
            { "GroupId", GroupId }
        };
}
