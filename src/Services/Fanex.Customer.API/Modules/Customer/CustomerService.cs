using Fanex.Customer.API.Modules.Customer.PositionTaking;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer;

public interface ICustomerService
{
    Task<CustomerPT> GetCustomerPT(GetCustomerPTRequest request);
}

public class CustomerService : ICustomerService
{
    private readonly IDynamicRepository dynamicRepository;

    public CustomerService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public Task<CustomerPT> GetCustomerPT(GetCustomerPTRequest request)
        => dynamicRepository.GetAsync<CustomerPT>(new GetCustomerPTInfoCriteria(Enumeration.FromValue<UserLevel>(request.CustLevel), request.CustId));
}