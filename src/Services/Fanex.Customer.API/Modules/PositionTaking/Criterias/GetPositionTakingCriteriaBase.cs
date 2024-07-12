using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTaking.Criterias;

public class GetPositionTakingCriteriaBase : CriteriaBase
{
    public int ProductId { get; set; }
    
    public int PageSize { get; set; }

    public int PageIndex { get; set; }

    public int TotalPages { get; set; }

    public byte SettingStatus { get; set; }

    public override string GetSettingKey() => "ViewPT_GetSportsbookPT_Super";

    public override bool IsValid() => true;
}