using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    public class SelectMemberBalanceCriteria : CriteriaBase
    {
        public SelectMemberBalanceCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey() => "Age_BalanceMemberSel";

        public override bool IsValid() => CustId > 0;
    }
}