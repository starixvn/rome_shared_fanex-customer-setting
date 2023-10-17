using Fanex.Customer.API.Shared.Constants;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class GetCustomerCriteria : CriteriaBase
{
    public GetCustomerCriteria(int custId, byte creationStatus, int roleId, int groupId = 0,int groupType = 0)
    {
        CustId = custId;
        GroupId = groupId;
        GroupType = groupType;
        CreationStatus = creationStatus;
        RoleId = roleId;
    }

    public int CustId { get; }

    public int GroupId { get; }

    public int GroupType { get; }

    public byte CreationStatus { get; }

    [SpParam(Ignored = true)]
    public int RoleId { get; }

    public override string GetSettingKey()
        => RoleId switch
        {
            ((int)CustomerLevel.Super) => "Acc_LiteMode_CustSetting_SuperAllInfoSel",
            ((int)CustomerLevel.Master) => "Acc_LiteMode_CustSetting_MasterAllInfoSel",
            ((int)CustomerLevel.Agent) => "Acc_LiteMode_CustSetting_AgentAllInfoSel",
            _ => throw new IndexOutOfRangeException($"RoleId cannot = {RoleId}")
        };

    public override bool IsValid() => CustId > 0 && GroupId >= 0 && GroupType >= 0;
}