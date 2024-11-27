using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType158;

public class GetGlobalStakeLevelCriteria : CriteriaBase
{
    public GetGlobalStakeLevelCriteria(int currency, string site, byte settingStatus, byte systemId, string userGroup)
    {
        Currency = currency;
        Site = site;
        SettingStatus = settingStatus;
        SystemID = systemId;
        GroupName = userGroup;
    }

    public int Currency { get; }

    public string Site { get; }

    public byte SettingStatus { get; }

    [SpParam(Ignored = true)]
    public byte SystemID { get; }

    [SpParam(Ignored = true)]
    public string GroupName { get; }

    public override bool IsValid() => SystemID >= 0;

    public override string GetSettingKey() => "Acc_CustSetting_GlobalStakeLevel_158_Select";
}
