using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class GetCashMemberBanksCriteria : CriteriaBase
{
    public GetCashMemberBanksCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey() => "Acc_CustomerSelBankAccount_CashMember";

    public override bool IsValid() => CustId > 0;
}