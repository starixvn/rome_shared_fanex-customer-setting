using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType152;
public class GroupType152Service : IGroupTypeBaseService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType152Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<GroupItemDefault>> Query(int uplineCustId, int productGroupId)
        => await dynamicRepository.FetchAsync<ParlayMaxPayoutSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = 152,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        });
}
