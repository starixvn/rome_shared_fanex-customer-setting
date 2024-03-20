using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class GetCreditCurrenciesCriteria : CriteriaBase
{
    private const int CreditCurrenciesMode = 1;

    public GetCreditCurrenciesCriteria()
    { }

    public int Mode => CreditCurrenciesMode;

    public override string GetSettingKey() => "Acc_ExchangeSel";

    public override bool IsValid()=> true;
}
