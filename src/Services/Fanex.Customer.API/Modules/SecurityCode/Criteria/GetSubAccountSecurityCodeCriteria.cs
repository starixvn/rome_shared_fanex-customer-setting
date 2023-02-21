using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SecurityCode.Criteria;

public class GetSubAccountSecurityCodeCriteria : CriteriaBase
{
    public GetSubAccountSecurityCodeCriteria(int custId, int subAccId)
    {
        CustId = custId;
        SubAccId = subAccId;
    }

    public int CustId { get; }

    public int SubAccId { get; }

    public override string GetSettingKey() => "Age_GetSubAccountSecurityCode";

    public override bool IsValid() => CustId > 0;
}