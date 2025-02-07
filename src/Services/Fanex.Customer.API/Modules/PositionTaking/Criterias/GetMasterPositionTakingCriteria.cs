namespace Fanex.Customer.API.Modules.PositionTaking.Criterias;

public class GetMasterPositionTakingCriteria : GetPositionTakingCriteriaBase
{
    public int SRecommend { get; set; }

    public override bool IsValid() => SRecommend > 0;

    public override string GetSettingKey() => "ViewPT_GetSportsbookPT_Super";
}