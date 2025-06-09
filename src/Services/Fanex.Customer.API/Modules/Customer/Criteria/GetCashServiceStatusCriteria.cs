using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCashServiceStatusCriteria : CriteriaBase
{
    public GetCashServiceStatusCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public byte ServiceType { get; set; }

    public override string GetSettingKey() => "Age_CashService_CustServiceTypeSel";

    public override bool IsValid() => true;
}
