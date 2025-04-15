using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTaking.Criterias;

public abstract class GetPositionTakingCriteriaBase : CriteriaBase
{
    public int ProductId { get; set; }

    public int PageSize { get; set; }

    public int PageIndex { get; set; }

    public int TotalPages { get; set; }

    public byte SettingStatus { get; set; }

    public override bool IsValid() => true;
}