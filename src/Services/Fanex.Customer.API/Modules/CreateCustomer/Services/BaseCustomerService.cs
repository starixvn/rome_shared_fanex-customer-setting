using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public class BaseCustomerService
{
    protected readonly IDynamicRepository dynamicRepository;
    protected readonly AppSettings appSettings;
    protected readonly ISharedService sharedService;
    protected readonly ICacheRefService cacheRefService;
    protected readonly IMessageService messageService;
    protected readonly Dictionary<int, dynamic> groupItemMapping;

    protected BaseCustomerService(
        IDynamicRepository dynamicRepository, 
        IOptions<AppSettings> appSettings, 
        ISharedService sharedService, 
        ICacheRefService cacheRefService,
        IMessageService messageService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.sharedService = sharedService;
        this.cacheRefService = cacheRefService;
        this.messageService = messageService;

        this.groupItemMapping = new()
        {
            {150, new GroupTypeBaseService<CommissionGroup>(dynamicRepository, appSettings.Value)},
            {151, new GroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings.Value)},
            {152, new GroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings.Value)},
            {153, new GroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings.Value)},
            {154, new GroupTypeBaseService<ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings.Value)},
            {155, new GroupTypeBaseService<WinlossLimitSettingItem>(dynamicRepository, appSettings.Value)},
            {156, new GroupTypeBaseService<CommissionSettingItem>(dynamicRepository, appSettings.Value)},
            {157, new GroupTypeBaseService<LossLimitSettingItem>(dynamicRepository, appSettings.Value)}
        };
    }
}
