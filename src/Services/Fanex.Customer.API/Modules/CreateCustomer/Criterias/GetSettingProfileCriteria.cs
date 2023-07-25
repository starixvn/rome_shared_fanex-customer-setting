using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criterias;

public class GetSettingProfileCriteria : CriteriaBase
{
    public byte CreationStatus { get; set; }

    public string Site { get; set; }

    public int Currency { get; set; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_ProductProfileSelect";

    public override bool IsValid() => true;
}
