using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;

public class GetSuperPTCriteria : CriteriaBase
{
    public GetSuperPTCriteria(int custId, int groupId, byte settingStatus)
    {
        CustId = custId;
        GroupId = groupId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public int GroupId { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey() => "Acc_CustSetting_SuperPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
