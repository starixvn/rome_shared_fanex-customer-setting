using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType154;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType154;
public class GroupType154Service : IGroupTypeBaseService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType154Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<GroupItemDefault>> Query(int uplineCustId, int productGroupId)
        => await dynamicRepository.FetchAsync<BetSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = 154,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        });
}
