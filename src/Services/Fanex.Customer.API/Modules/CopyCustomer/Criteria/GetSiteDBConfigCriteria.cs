using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Criteria;
public class GetSiteDBConfigCriteria : CriteriaBase
    {
        public GetSiteDBConfigCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey() => "Acc_SiteDBConfigSel";

        public override bool IsValid() => CustId > 0;
    }