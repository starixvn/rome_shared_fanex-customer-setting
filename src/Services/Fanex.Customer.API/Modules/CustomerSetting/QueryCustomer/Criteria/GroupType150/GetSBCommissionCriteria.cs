using System.Collections;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType150;

public class GetSBCommissionCriteria : CriteriaBase
{
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

    public override string GetSettingKey() => "Acc_CustSetting_Discount150Sel";

    public override bool IsValid() => CustId > 0;

    public Hashtable GetParameters()
        => new Hashtable
        {
            { "CustId", CustId },
            { "UplineRoleId", UplineRoleId },
            { "SettingStatus", SettingStatus },
        };

}
