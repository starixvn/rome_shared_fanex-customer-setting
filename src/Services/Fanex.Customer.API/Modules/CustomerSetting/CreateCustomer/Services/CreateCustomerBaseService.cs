using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;

public interface ICreateCustomerBaseService
{
    Task<CreateCustomerResult> Create(CreateRequest request);
}

public class CreateCustomerBaseService: ICreateCustomerBaseService
{
    protected readonly IMessageService messageService;

    protected readonly AppSettings appSettings;

    protected readonly ICustomerSettingHelper helperService;

    public CreateCustomerBaseService(IMessageService messageService, 
        AppSettings appSettings, 
        ICustomerSettingHelper helperService)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
        this.helperService = helperService;
    }

    public virtual Task<CreateCustomerResult> Create(CreateRequest request)
        => throw new NotImplementedException();
}
