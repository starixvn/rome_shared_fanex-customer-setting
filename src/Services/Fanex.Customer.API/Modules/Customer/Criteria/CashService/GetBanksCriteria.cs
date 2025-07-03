using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;
public class GetBanksCriteria(string site) : CriteriaBase
{
    public string Site { get; } = site;

    public override string GetSettingKey() => "Age_CashService_BankList";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Site);
}