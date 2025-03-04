using Fanex.Data.Repository;

namespace BetLimitControl.API.Features.MaxBetControl.Criteria
{
    public class GetCustomerMultipliersBySiteCriteria(int siteId, bool isTest) : CriteriaBase
    {
        public int SiteId { get; } = siteId;

        public bool IsTest { get; } = isTest;

        public override string GetSettingKey() => "API_BetLimitIncrease_GetListCust";

        public override bool IsValid() => SiteId > 0;
    }
}