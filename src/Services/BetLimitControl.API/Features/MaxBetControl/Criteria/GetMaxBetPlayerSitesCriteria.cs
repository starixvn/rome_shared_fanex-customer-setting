using Fanex.Data.Repository;

namespace BetLimitControl.API.Features.MaxBetControl.Criteria
{
    public class GetMaxBetPlayerSitesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "API_SVIPMaxBetPlayer_GetSite";

        public override bool IsValid() => true;
    }
}