using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class GetSettingProfileCriteria : CriteriaBase
{
    public GetSettingProfileCriteria(byte creationStatus, string site, int currency)
    {
        CreationStatus = creationStatus;
        Site = site;
        Currency = currency;
    }
    
    public byte CreationStatus { get; }

    public string Site { get; }

    public int Currency { get; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_ProductProfileSelect";

    public override bool IsValid() => true;
}
