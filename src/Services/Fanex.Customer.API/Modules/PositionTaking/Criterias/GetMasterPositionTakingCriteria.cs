namespace Fanex.Customer.API.Modules.PositionTaking.Criterias;

public class GetMasterPositionTakingCriteria : GetPositionTakingCriteriaBase
{
    public int SRecommend { get; set; }

    public override bool IsValid() => SRecommend > 0;
}