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

    Task<CreditInfo> GetCreditInfo(string username);

    Task<string> GetSuggestUsername(GetSuggestUsernameRequest request);

    Task<bool> CheckExistedCustomer(string username);

    Task<Downline> GetDownlineByUsername(int custId, int subAccountId, string username);

    Task<bool> IsEnableCustomer(int custId);

    Task<CustomerStatus> GetCustomerStatus(int custId, int subAccId);
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

    public Task<CreditInfo> GetCreditInfo(string username)
        => dynamicRepository.GetAsync<CreditInfo>(new GetCustomerMaxCreditCriteria(username));

    public async Task<string> GetSuggestUsername(GetSuggestUsernameRequest request)
    {
        var command = new GetSuggestUsernameCommand(request.ActorName, request.RoleId);
        await dynamicRepository.ExecuteAsync(command);
        return command.SeqName;
    }

    public async Task<bool> CheckExistedCustomer(string username)
    {
        var command = new CheckExistedCustomerCommand(username);
        await dynamicRepository.ExecuteAsync(command);
        return command.IsExists;
    }

    public async Task<Downline> GetDownlineByUsername(int custId, int subAccountId, string username)
    {
        var command = new CheckBelongingByUsernameCommand(custId, username, subAccountId);
        await dynamicRepository.ExecuteAsync(command);
        return new(command.IsActive, command.CustId, command.IsSubAcc, command.RoleId);
    }

    public async Task<bool> IsEnableCustomer(int custId)
    {
        var command = new CheckEnableCustomerCommand(custId);
        await dynamicRepository.ExecuteAsync(command);
        return command.ErrCode.Equals(0);
    }

    public async Task<CustomerStatus> GetCustomerStatus(int custId, int subAccId)
    {
        var customerStatusParam = $"<r id=\"{custId}\" sub=\"{subAccId}\"/>";
        var custList = $"<Root>{customerStatusParam}</Root>";
        return (await dynamicRepository.FetchAsync<CustomerStatus>(new GetCustomerStatusCriteria(custList))).FirstOrDefault();
    }
}