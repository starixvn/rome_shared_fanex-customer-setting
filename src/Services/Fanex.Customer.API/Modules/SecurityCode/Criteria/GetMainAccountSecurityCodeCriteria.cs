using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SecurityCode.Criteria;

public class GetMainAccountSecurityCodeCriteria : CriteriaBase
{
    public GetMainAccountSecurityCodeCriteria(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public override string GetSettingKey() => "Age_GetMainAccountSecurityCode";

    public override bool IsValid() => CustId > 0;
}