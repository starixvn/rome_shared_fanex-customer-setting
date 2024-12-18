using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;
using Fanex.Customer.API.Modules.EditCustomer;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Password.Service;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public class CreateCustomerServiceBase : PasswordServiceBase
{
    protected readonly AppSettings appSettings;
    protected readonly ISettingProfileService settingProfileService;
    protected readonly ICacheRefService cacheRefService;
    protected readonly IMessageService messageService;
    protected readonly IUMService uMService;
    protected readonly ICustomerSettingHelper helperService;

    public CreateCustomerServiceBase(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISettingProfileService settingProfileService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        IUMService uMService,
        ICustomerSettingHelper helperService,
        IEncryption encryption,
        HttpClient httpClient)
        : base(dynamicRepository, encryption, appSettings.Value, httpClient)
    {
        this.uMService = uMService;
        this.messageService = messageService;
        this.settingProfileService = settingProfileService;
        this.helperService = helperService;
        this.appSettings = appSettings.Value;
        this.cacheRefService = cacheRefService;
    }
}