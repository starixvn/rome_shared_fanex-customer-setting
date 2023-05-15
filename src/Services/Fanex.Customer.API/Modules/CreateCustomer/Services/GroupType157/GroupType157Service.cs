using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType157;
public class GroupType157Service : IGroupTypeBaseService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType157Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<GroupItemDefault>> Query(int uplineCustId, int productGroupId)
        => await dynamicRepository.FetchAsync<LossLimitSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = 157,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        });
}
