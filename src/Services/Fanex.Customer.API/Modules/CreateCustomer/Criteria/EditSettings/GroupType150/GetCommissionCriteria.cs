using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType150;

public class GetCommissionCriteria : CriteriaBase
{
    private readonly Dictionary<int, string> SPNamesMapping = new()
    {
        {150_1, "Acc_CustSetting_Discount150Sel_Member"},
        {150_2, "Acc_CustSetting_Discount150Sel_Agent"},
        {150_3, "Acc_CustSetting_Discount150Sel_Master"},
        {150_4, "Acc_CustSetting_Discount150Sel_Super"}
    };
    public int CustId { get; set; }

    public int UplineRoleId { get; set; }

    public byte SettingStatus { get; set; }

    [SpParam(Ignored = true)]
    public int RoleId { get; set; }

    public override string GetSettingKey()
        => SPNamesMapping[150 * 10 + (RoleId)];

    public override bool IsValid() => CustId > 0;
}
