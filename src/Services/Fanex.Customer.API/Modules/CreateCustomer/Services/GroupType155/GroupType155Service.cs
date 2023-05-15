using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType155;
public class GroupType155Service : IGroupTypeBaseService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType155Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<GroupItemDefault>> Query(int uplineCustId, int productGroupId)
        => await dynamicRepository.FetchAsync<WinlossLimitSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = 155,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        });
}
