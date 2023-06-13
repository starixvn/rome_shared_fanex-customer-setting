using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.GroupType152;

public interface IGroupType152Service<ParlayMaxPayoutSettingItem>
{
    Task<IEnumerable<ParlayMaxPayoutSettingItem>> Query(int uplineCustId, int productGroupId, int groupType);
}

public class GroupType152Service
{
    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    public GroupType152Service(IDynamicRepository dynamicRepository, AppSettings appSettings)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
    }

    public async Task<IEnumerable<ParlayMaxPayoutSettingItem>> Query(int uplineCustId, int productGroupId, int groupType)
    => (await dynamicRepository.FetchAsync<ParlayMaxPayoutSettingItem>(new GetCustomerCriteria
        {
            GroupId = productGroupId,
            GroupType = groupType,
            CustId = uplineCustId,
            CreationStatus = appSettings.CreationStatus
        }))
        .Select(i =>
        {
            i.Combo2 = i.MaxCombo2;
            i.Combo35 = i.MaxCombo35;
            i.ComboOver6 = i.MaxComboOver6;
            i.SingleMatchParlay = i.MaxSingleMatchParlay;
            return i;
        });

}
