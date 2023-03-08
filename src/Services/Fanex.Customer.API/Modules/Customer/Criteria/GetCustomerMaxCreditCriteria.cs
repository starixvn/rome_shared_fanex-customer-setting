using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCustomerMaxCreditCriteria : CriteriaBase
{
    public GetCustomerMaxCreditCriteria(string username)
    {
        Username = username;
    }

    [SpParam(SqlParamName = "RefCust")]
    public string Username { get; }

    public override string GetSettingKey() => "Acc_CustomerCreditInfoSel";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);
}