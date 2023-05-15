using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType151;
public class GroupType151Service : IGroupTypeBaseService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType151Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<GroupItemDefault>> Query(int uplineCustId, int productGroupId)
        => await dynamicRepository.FetchAsync<BetSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = 151,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        });
}