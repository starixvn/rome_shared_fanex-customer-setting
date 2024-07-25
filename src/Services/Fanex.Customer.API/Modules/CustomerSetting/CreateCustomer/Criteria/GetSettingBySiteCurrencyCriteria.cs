using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class GetSettingBySiteCurrencyCriteria : CriteriaBase
{
    public GetSettingBySiteCurrencyCriteria(string site, int currency, int groupId, int groupType, byte settingStatus)
    {
        Currency = currency;
        Site = site;
        GroupId = groupId;
        GroupType = groupType;
        SettingStatus = settingStatus;
    }

    public string Site { get; }

    public int Currency { get; }

    public int GroupId { get; }

    public int GroupType { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey() => "Acc_CustSetting_SiteCurrencySettingInfoSel";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Site) && Currency > 0;

}
