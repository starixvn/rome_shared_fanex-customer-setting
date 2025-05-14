using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerBaseService
{
    Task<CreateCustomerResult> Create(CreateRequest request);
}

public class CreateCustomerBaseService: ICreateCustomerBaseService
{
    protected readonly IMessageService messageService;

    protected readonly AppSettings appSettings;

    protected readonly ICustomerSettingHelper helperService;

    protected readonly IDynamicRepository dynamicRepository;

    public CreateCustomerBaseService(IMessageService messageService, 
        AppSettings appSettings, 
        ICustomerSettingHelper helperService,
        IDynamicRepository dynamicRepository)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
        this.helperService = helperService;
        this.dynamicRepository = dynamicRepository;
    }

    public virtual Task<CreateCustomerResult> Create(CreateRequest request)
        => throw new NotImplementedException();
}
