using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<CustomerSetting> Query(int custId);

}

public class CreateCustomerService : ICreateCustomerService
{
    private readonly IDynamicRepository dynamicRepository;
    
    public CreateCustomerService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }
    public async Task<CustomerSetting> Query(int custId)
    => new CustomerSetting
    {
        GeneralInfo = await dynamicRepository.GetAsync<GeneralInformation>(new GetCustomerCriteria { CustId = custId, GroupId = 0, GroupType = 0 }),
    };
}