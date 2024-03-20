using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class GetHeadlineBelongSiteCriteria : CriteriaBase
{
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Acc_CustomerInsMember_SiteHeadlineSel";
}
