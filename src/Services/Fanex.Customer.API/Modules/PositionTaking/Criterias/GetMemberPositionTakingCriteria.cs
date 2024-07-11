namespace Fanex.Customer.API.Modules.PositionTaking.Criterias;

public class GetMemberPositionTakingCriteria : GetPositionTakingCriteriaBase
{
    public int Recommend { get; set; }

    public override string GetSettingKey() => "ViewPT_GetSportsbookPT_Agent";

    public override bool IsValid() => Recommend > 0;
}