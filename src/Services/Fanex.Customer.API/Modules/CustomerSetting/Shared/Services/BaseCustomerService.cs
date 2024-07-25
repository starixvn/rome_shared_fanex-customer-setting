using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.EditCustomer;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;

public class BaseCustomerService
{
    protected readonly IDynamicRepository dynamicRepository;
    protected readonly AppSettings appSettings;
    protected readonly ISettingProfileService settingProfileService;
    protected readonly ICacheRefService cacheRefService;
    protected readonly IMessageService messageService;
    protected readonly IUMService uMService;
    protected readonly ICustomerSettingHelper helperService;

    public BaseCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISettingProfileService settingProfileService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        IUMService uMService,
        ICustomerSettingHelper helperService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.settingProfileService = settingProfileService;
        this.cacheRefService = cacheRefService;
        this.messageService = messageService;
        this.uMService = uMService;
        this.helperService = helperService;
    }
}
