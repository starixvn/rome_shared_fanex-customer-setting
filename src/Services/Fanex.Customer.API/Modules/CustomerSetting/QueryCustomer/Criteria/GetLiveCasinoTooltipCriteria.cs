using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Criteria;

public class GetLiveCasinoTooltipCriteria : CriteriaBase
{
    public string Site { get; }
    public int Currency { get; }
    public int SettingStatus { get; }

    public GetLiveCasinoTooltipCriteria(string site, int currency, int settingStatus)
    {
        Site = site;
        Currency = currency;
        SettingStatus = settingStatus;
    }

    public override bool IsValid() => !string.IsNullOrEmpty(Site) && Currency > 0;
    public override string GetSettingKey() => "Acc_CustSetting_GetTooltipProduct";
}