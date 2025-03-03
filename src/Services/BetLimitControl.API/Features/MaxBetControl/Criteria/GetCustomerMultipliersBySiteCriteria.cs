using Fanex.Data.Repository;

namespace BetLimitControl.API.Features.MaxBetControl.Criteria
{
    public class GetCustomerMultipliersBySiteCriteria(int siteId) : CriteriaBase
    {
        public int SiteId { get; } = siteId;

        public override string GetSettingKey() => "API_BetLimitIncrease_GetListCust";

        public override bool IsValid() => SiteId > 0;
    }
}