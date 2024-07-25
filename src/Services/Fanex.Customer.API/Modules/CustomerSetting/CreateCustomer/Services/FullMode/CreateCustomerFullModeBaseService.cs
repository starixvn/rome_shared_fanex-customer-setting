using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomrSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;

public interface ICreateCustomerFullModeBaseService
{
    Task<CreateCustomerResult> Create(CreateRequest request);
}
public class CreateCustomerFullModeBaseService: ICreateCustomerFullModeBaseService
{
    protected readonly IScaleoutDBService scaleoutDBService;

    protected readonly IMessageService messageService;

    protected readonly AppSettings appSettings;

    protected readonly ICustomerSettingHelper helperService;

    public CreateCustomerFullModeBaseService(IMessageService messageService, 
        AppSettings appSettings, 
        IScaleoutDBService scaleoutDBService,
        ICustomerSettingHelper helperService)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
        this.scaleoutDBService = scaleoutDBService;
        this.helperService = helperService;
    }

    public virtual Task<CreateCustomerResult> Create(CreateRequest request)
        => throw new NotImplementedException();
}
