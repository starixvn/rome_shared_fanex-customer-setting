using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    public class SelectMemberBalanceBySiteDBCriteria : NonQueryCommand
    {
        public SelectMemberBalanceBySiteDBCriteria(int custId, string siteDBName)
        {
            CustId = custId;
            SiteDBName = siteDBName;
        }

        public int CustId { get; }

        [SpParam(Ignored = true)]
        public string SiteDBName { get; }

        public decimal Balance { get; set; }

        public decimal NetPosition { get; set; }

        public decimal PendingTransfer3rd { get; set; }

        public override string GetSettingKey() => $"{SiteDBName}_Acc_BalanceMemberSel";

        public override bool IsValid() => CustId > 0;
    }
}