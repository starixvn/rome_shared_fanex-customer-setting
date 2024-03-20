using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings;

public class GetCustomerForUpdateCriteria : CriteriaBase
{
    private readonly Dictionary<int, string> SPNamesMapping = new()
    {
        {151, "Acc_CustSetting_BetSetting151Sel"},
        {152, "Acc_CustSetting_MaxPayoutSelect"}
    };

    public GetCustomerForUpdateCriteria(int custId, byte settingStatus, int roleId, int groupId = 0, int groupType = 0)
    {
        CustId = custId;
        GroupId = groupId;
        GroupType = groupType;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]// Tam ignore
    public int GroupId { get; }

    [SpParam(Ignored = true)]// Tam ignore
    public int GroupType { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey()
        => SPNamesMapping[GroupType];

    public override bool IsValid() => CustId > 0 && GroupId >= 0 && GroupType >= 0;
}