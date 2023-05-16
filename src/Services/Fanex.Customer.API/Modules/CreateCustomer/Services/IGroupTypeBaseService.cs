using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface IGroupTypeBaseService<T>
{
    Task<IEnumerable<T>> Query(int uplineCustId, int productGroupId, int groupType);
}

public class GroupTypeBaseService<T>
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;
    
    public GroupTypeBaseService() : this (null, null)
    {

    }

    public GroupTypeBaseService(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<T>> Query(int uplineCustId, int productGroupId, int groupType)
        => await dynamicRepository.FetchAsync<T>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = groupType,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        });
}