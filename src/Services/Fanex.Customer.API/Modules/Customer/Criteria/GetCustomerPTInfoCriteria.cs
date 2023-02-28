using Fanex.Customer.API.Shared.Models;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCustomerPTInfoCriteria : CriteriaBase
{
    public GetCustomerPTInfoCriteria(UserLevel custLevel, int custId)
    {
        CustLevel = custLevel;
        CustId = custId;
    }

    [SpParam(Ignored = true)]
    public UserLevel CustLevel { get; }

    public int CustId { get; }

    public override string GetSettingKey()
        => CustLevel.IsAgentLevel()
        ? "Acc_CustomerSelAgentInfo"
        : CustLevel.IsMasterLevel()
        ? "Acc_CustomerSelMAgentInfo"
        : "Acc_CustomerSelSAgentInfo";

    public override bool IsValid()
        => CustId > 0;
}