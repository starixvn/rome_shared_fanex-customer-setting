namespace Fanex.Customer.API.Modules.PositionTaking.Criterias;

public class GetMemberPositionTakingCriteria : GetPositionTakingCriteriaBase
{
    public int UplineId { get; set; }

    public override string GetSettingKey() => "ViewPT_GetSportsbookPT_Agent";

    public override bool IsValid() => UplineId > 0;
}