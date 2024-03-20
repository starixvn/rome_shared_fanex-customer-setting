using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings;

public abstract class GetCustomerForUpdateCriteria : CriteriaBase
{
    public GetCustomerForUpdateCriteria(int custId, byte settingStatus, int roleId, int groupId = 0, int groupType = 0)
    {
        CustId = custId;
        GroupId = groupId;
        GroupType = groupType;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public int GroupId { get; }

    [SpParam(Ignored = true)]
    public int GroupType { get; }

    public byte SettingStatus { get; }

    public override bool IsValid() => CustId > 0 && GroupId >= 0 && GroupType >= 0;
}