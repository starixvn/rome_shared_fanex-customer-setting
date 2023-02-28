using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.Customer;

public interface ICustomerService
{
    Task<CustomerPT> GetCustomerPT(int custId, byte custLevel);

    Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param);

    Task<CustomerInfo> GetCustomerInfo(int custId);
}

public class CustomerService : ICustomerService
{
    private readonly AppSettings appSettings;
    private readonly IDynamicRepository dynamicRepository;

    public CustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
    }

    public async Task<CustomerPT> GetCustomerPT(int custId, byte custLevel)
        => await dynamicRepository.GetAsync<CustomerPT>(new GetCustomerPTInfoCriteria(Enumeration.FromValue<UserLevel>(custLevel), custId));

    public async Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param)
    {
        var criteria = new SearchCustomerCriteria(param.CustId, param.CustId, param.Query, param.IsSubAccount, param.ExcludeMember, param.DirectDownLine, param.IsAccInfo, param.IsInternalAgent);
        return await dynamicRepository.FetchAsync<SuggestCustomer>(criteria);
    }

    public async Task<CustomerInfo> GetCustomerInfo(int custId)
        => await dynamicRepository.GetAsync<CustomerInfo>(new GetCustomerInfoCriteria(custId, appSettings.SettingStatus));
}