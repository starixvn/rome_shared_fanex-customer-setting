using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCustomerMaxCreditCriteria : CriteriaBase
{
    public GetCustomerMaxCreditCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey() => "Acc_CustomerCreditInfoSel";

    public override bool IsValid() => CustId > 0;
}