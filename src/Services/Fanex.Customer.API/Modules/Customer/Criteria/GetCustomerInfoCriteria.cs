using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCustomerInfoCriteria : CriteriaBase
{
    public int CustId { get; init; }

    public override string GetSettingKey() => "Acc_CustSetting_CustomerInfoSBSelect";

    public override bool IsValid() => CustId > 0;
}

