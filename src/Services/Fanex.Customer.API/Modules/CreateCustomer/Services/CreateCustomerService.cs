using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType159;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.Resources;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<Result> Create(CreateRequest request);
}

public class CreateCustomerService : ICreateCustomerService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly AppSettings appSettings;
    private readonly ISharedService sharedService;
    private readonly ICacheRefService cacheRefService;
    private readonly Dictionary<int, dynamic> groupItemMapping;

    public CreateCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ISharedService sharedService, ICacheRefService cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.sharedService = sharedService;
        this.cacheRefService = cacheRefService;

        this.groupItemMapping = new()
        {
            {150, new GroupTypeBaseService<CommissionGroup>(dynamicRepository, appSettings.Value)},
            {151, new GroupTypeBaseService<BetSettingItem>(dynamicRepository, appSettings.Value)},
            {152, new GroupTypeBaseService<ParlayMaxPayoutSettingItem>(dynamicRepository, appSettings.Value)},
            {153, new GroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings.Value)},
            {154, new GroupTypeBaseService<ViewModels.GroupType154.BetSettingItem>(dynamicRepository, appSettings.Value)},
            {155, new GroupTypeBaseService<WinlossLimitSettingItem>(dynamicRepository, appSettings.Value)},
            {156, new GroupTypeBaseService<CommissionSettingItem>(dynamicRepository, appSettings.Value)},
            {157, new GroupTypeBaseService<LossLimitSettingItem>(dynamicRepository, appSettings.Value)},
            {158, new GroupTypeBaseService<ViewModels.GroupType158.CommissionSettingItem>(dynamicRepository, appSettings.Value)},
            {159, new GroupTypeBaseService<BetsettingItem>(dynamicRepository, appSettings.Value)},
            {160, new GroupTypeBaseService<SuperPTSettingItem>(dynamicRepository, appSettings.Value)}
        };
    }

    public async Task<Result> Create(CreateRequest request)
    {
        return new Result();
    }
}