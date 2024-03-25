using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCommissionHintCriteria : CriteriaBase
{
    public GetCommissionHintCriteria(int custId, string site)
    {
        CustId = custId;
        Site = site;
    }

    public int CustId { get; }
    
    public string Site { get; }

    public override bool IsValid()
        => CustId >= 0;

    public override string GetSettingKey()
        => "Age_MemCommission_SelBettype";
}
