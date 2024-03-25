using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICurrencyService
{
    Task<IEnumerable<Currency>> GetCreditCurrencies();
}

public class CurrencyService : ICurrencyService
{
    private readonly IDynamicRepository dynamicRepository;

    public CurrencyService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
    }

    public async Task<IEnumerable<Currency>> GetCreditCurrencies()
        => await this.dynamicRepository.FetchAsync<Currency>(new GetCreditCurrenciesCriteria());
}
