using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class GetCountryCodesCriteria : CriteriaBase
{
    public GetCountryCodesCriteria(string site)
    {
        Site = site;
    }

    [SpParam(SqlParamName = "site")]
    public string Site { get; }

    public override string GetSettingKey() => "Age_CashService_CountryCodeSel";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Site);
}