using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public class GetSiteDBConfigByCustCriteria(int custId) : CriteriaBase
    {
        public int CustId { get; } = custId;

        public override string GetSettingKey() => "Acc_Dep_ServerDBConfigSelByCust";

        public override bool IsValid() => CustId > 0;
    }
}