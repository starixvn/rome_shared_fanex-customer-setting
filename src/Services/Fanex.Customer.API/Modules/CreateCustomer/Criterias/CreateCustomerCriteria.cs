using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criterias;

public class CreateCustomerCriteria : CriteriaBase
{

    public override string GetSettingKey() => "Acc_LiteMode_CustomerInsMember_SAg";

    public override bool IsValid() => true;
}
