using Fanex.Data;
using Fanex.Data.Repository;
namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetDomainsForCashMemberCriteria: CriteriaBase
{
    public GetDomainsForCashMemberCriteria(int uplineCustId)
        => UplineCustId = uplineCustId;

    [SpParam(SqlParamName = "CustId")]
    public int UplineCustId { get; }

    public override string GetSettingKey()
        => "Age_CashService_GeneralSetting";

    public override bool IsValid() => true;
}