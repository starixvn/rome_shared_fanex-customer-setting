using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType150;

public class GetSBCommissionCriteria : CriteriaBase
{
    private readonly Dictionary<int, string> SPNamesMapping = new()
    {
        {1, "Acc_CustSetting_Discount150Sel_Member"},
        {2, "Acc_CustSetting_Discount150Sel_Agent"},
        {3, "Acc_CustSetting_Discount150Sel_Master"},
        {4, "Acc_CustSetting_Discount150Sel_Super"}
    };

    public GetSBCommissionCriteria(int custId, byte roleId, int uplineRoleId, byte settingStatus)
    {
        CustId = custId;
        RoleId = roleId;
        UplineRoleId = uplineRoleId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public int UplineRoleId { get; }

    public byte SettingStatus { get; }

    [SpParam(Ignored = true)]
    public byte RoleId { get; }

    public override string GetSettingKey() => SPNamesMapping[RoleId];

    public override bool IsValid() => CustId > 0;
}
