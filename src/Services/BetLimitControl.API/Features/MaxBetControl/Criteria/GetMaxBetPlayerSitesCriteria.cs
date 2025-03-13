using Fanex.Data.Repository;

namespace BetLimitControl.API.Features.MaxBetControl.Criteria
{
    public class GetMaxBetPlayerSitesCriteria(bool isTest) : CriteriaBase
    {
        public bool IsTest { get; } = isTest;

        public override string GetSettingKey() => "API_BetLimitIncrease_GetSite";

        public override bool IsValid() => true;
    }
}