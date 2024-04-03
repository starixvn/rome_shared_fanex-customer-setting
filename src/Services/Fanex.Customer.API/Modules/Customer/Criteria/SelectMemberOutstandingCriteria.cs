using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    public class SelectMemberOutstandingCriteria : NonQueryCommand
    {
        public SelectMemberOutstandingCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public decimal Outstanding { get; set; }

        public override string GetSettingKey() => "Acc_OutstandingMemberSel";

        public override bool IsValid() => CustId > 0;
    }
}