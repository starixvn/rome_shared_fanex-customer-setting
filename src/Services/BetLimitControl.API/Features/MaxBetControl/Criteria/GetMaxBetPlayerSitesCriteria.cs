using Fanex.Data.Repository;

namespace BetLimitControl.API.Features.MaxBetControl.Criteria
{
    public class GetMaxBetPlayerSitesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "API_BetLimitIncrease_GetSite";

        public override bool IsValid() => true;
    }
}