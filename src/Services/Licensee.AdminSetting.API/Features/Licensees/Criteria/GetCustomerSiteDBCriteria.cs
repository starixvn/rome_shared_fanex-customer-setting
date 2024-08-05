using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class GetCustomerSiteDBCriteria(int siteDBGroupId) : CriteriaBase
    {
        public int SiteDBGroupId { get; } = siteDBGroupId;

        public override string GetSettingKey() => "API_SiteBuilder_GetCustomerSiteDB";

        public override bool IsValid() => true;
    }
}