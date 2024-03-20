using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetSabaSportsCriteria : CriteriaBase
{
    public GetSabaSportsCriteria(string siteName)
    {
        SiteName = siteName;
    }

    [SpParam(SqlParamName = "site")]
    public string SiteName { get; }

    public override bool IsValid()
        => true;

    public override string GetSettingKey()
        => "Acc_SabaVirtualSportsBetSettingRef_Sel";
}
