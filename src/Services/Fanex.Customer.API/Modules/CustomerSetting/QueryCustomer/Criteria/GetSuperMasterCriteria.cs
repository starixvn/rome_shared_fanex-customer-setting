using Fanex.Customer.API.Shared.Constants;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria;

public class GetSuperMasterCriteria : GetAgentCriteria
{
    public GetSuperMasterCriteria(int custId, byte settingStatus, bool? isDirectMember, int roleId, int groupId = 0, int groupType = 0)
        : base(custId, settingStatus, roleId, groupId, groupType)
    {
        IsDirectMember = isDirectMember;
    }

    public bool? IsDirectMember { get; }

    public override string GetSettingKey()
        => RoleId switch
        {
            CustomerLevel.Super => "Acc_LiteMode_CustSetting_SuperAllInfoSel",
            CustomerLevel.Master => "Acc_LiteMode_CustSetting_MasterAllInfoSel",
            _ => throw new IndexOutOfRangeException($"RoleId cannot = {RoleId}")
        };

    public override bool IsValid() => CustId > 0 && GroupId >= 0 && GroupType >= 0;
}