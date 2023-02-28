using Fanex.Customer.API.Modules.Customer.PositionTaking;
using Fanex.Customer.API.Modules.Customer.SearchCustomer;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer;

public interface ICustomerService
{
    Task<CustomerPT> GetCustomerPT(int custId, byte custLevel);

    Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param);
}

public class CustomerService : ICustomerService
{
    private readonly IDynamicRepository dynamicRepository;

    public CustomerService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<CustomerPT> GetCustomerPT(int custId, byte custLevel)
        => await dynamicRepository.GetAsync<CustomerPT>(new GetCustomerPTInfoCriteria(Enumeration.FromValue<UserLevel>(custLevel), custId));

    public async Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param)
    {
        var criteria = new SearchCustomerCriteria(param.CustId, param.CustId, param.Query, param.IsSubAccount, param.ExcludeMember, param.DirectDownLine, param.IsAccInfo, param.IsInternalAgent);
        return await dynamicRepository.FetchAsync<SuggestCustomer>(criteria);
    }
}