using Fanex.Data.Repository;

namespace Alpha.CloneAccount.Criteria;

public class GetMemberSportbooksSettingCriteria : CriteriaBase
{
    public GetMemberSportbooksSettingCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey()
        => "Acc_CustomerSelEditCustomer";

    public override bool IsValid()
        => CustId > 0;
}