using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType155;

public interface IGroupType155Service<WinlossLimitSettingItem>
{
    Task<IEnumerable<WinlossLimitSettingItem>> Query(int uplineCustId, int productGroupId, int groupType);
}

public class GroupType155Service
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType155Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<WinlossLimitSettingItem>> Query(int uplineCustId, int productGroupId, int groupType)
    => (await dynamicRepository.FetchAsync<WinlossLimitSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = groupType,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        }))
        .Select(i =>
        {
            i.WinLimit = i. MaxWinRange;
            i.LossLimit = i.MaxLossRange;
            i.IsAutoReset = i.IsAutoResetUpline;
            return i;
        });

}
