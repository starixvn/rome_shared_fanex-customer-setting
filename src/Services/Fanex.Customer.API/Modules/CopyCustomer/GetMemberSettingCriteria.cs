using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class GetMemberSettingCriteria : CriteriaBase
{
    public GetMemberSettingCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey()
        => "Acc_CustomerSelEditCustomer";

    public override bool IsValid()
        => CustId > 0;
}