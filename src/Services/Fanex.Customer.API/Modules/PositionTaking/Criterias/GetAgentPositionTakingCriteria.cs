namespace Fanex.Customer.API.Modules.PositionTaking.Criterias;

public class GetAgentPositionTakingCriteria : GetPositionTakingCriteriaBase
{
    public int MRecommend { get; set; }

    public override string GetSettingKey() => "ViewPT_GetSportsbookPT_Master";

    public override bool IsValid() => MRecommend > 0;
}