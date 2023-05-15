using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType153;

public class GroupType153Service : IGroupTypeBaseService
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType153Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<GroupItemDefault>> Query(int uplineCustId, int productGroupId)
        => await dynamicRepository.FetchAsync<SuperPTSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = 153,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        });
}
