using Fanex.Data.Repository;
namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCashServiceTotalDownlineCriteria: CriteriaBase
{
    public GetCashServiceTotalDownlineCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey() => "Age_CashService_TotalDownline";

    public override bool IsValid() => true;
}