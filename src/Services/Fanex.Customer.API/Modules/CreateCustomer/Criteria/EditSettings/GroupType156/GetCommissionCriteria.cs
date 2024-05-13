using System.Text.RegularExpressions;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType156;

public class GetCommissionCriteria : CriteriaBase
{
    private readonly Dictionary<int, string> SPNamesMapping = new()
    {
        {1, "Acc_CustSetting_Discount156_Sel_Member"},
        {2, "Acc_CustSetting_Discount156_Sel_Agent"},
        {3, "Acc_CustSetting_Discount156_Sel_Master"},
        {4, "Acc_CustSetting_Discount156_Sel_Super"}
    };

    public GetCommissionCriteria(int custId, int groupId, byte roleId, int uplineRoleId, byte settingStatus)
    {
        CustId = custId;
        GroupId = groupId;
        RoleId = roleId;
        UplineRoleId = uplineRoleId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public int UplineRoleId { get; }

    public byte SettingStatus { get; }

    public int GroupId { get; }

    [SpParam(Ignored = true)]
    public byte RoleId { get; }

    public override string GetSettingKey() => SPNamesMapping[RoleId];

    public override bool IsValid() => CustId > 0;
}
