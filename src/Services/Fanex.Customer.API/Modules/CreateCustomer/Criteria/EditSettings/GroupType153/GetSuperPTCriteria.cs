using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;

public class GetSuperPTCriteria : CriteriaBase
{
    // public GetSuperPTCriteria(int custId, byte settingStatus)
    // {
    //     CustId = custId;
    //     SettingStatus = settingStatus;
    // }

    public int CustId { get; set;}

    public byte SettingStatus { get; set;}

    public override string GetSettingKey()
        => "Acc_CustSetting_SuperPT_153_Select";

    public override bool IsValid() => CustId > 0;
}
