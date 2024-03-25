using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetProductCategoriesCriteria : CriteriaBase
{
    public GetProductCategoriesCriteria(byte settingStatus, string site, int? custId, int? currency)
    {
        CustId = custId;
        SettingStatus = settingStatus;
        Site = site;
        Currency = currency;
    }

    public byte SettingStatus { get; set; }

    public int? CustId { get; set; }

    public string Site { get; set; }

    public int? Currency { get; init; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_ProductListInfoSel";

    public override bool IsValid() => true;
}