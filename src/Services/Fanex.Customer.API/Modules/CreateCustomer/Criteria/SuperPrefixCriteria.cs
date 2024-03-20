using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class SuperPrefixCriteria : CriteriaBase
{
        public string GroupName { get; set; }

        public string Prefix { get; set; }
        
        public bool? AllowNewSuper { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Acc_SuperPrefixByGroupSel";
}
