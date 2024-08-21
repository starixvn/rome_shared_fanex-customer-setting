using Fanex.Customer.API;
using Fanex.Customer.API.Modules.CopyCustomer;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;

public abstract class CopyCustomerBaseService
{
    protected readonly IMessageService messageService;

    protected readonly AppSettings appSettings;

    public CopyCustomerBaseService(IMessageService messageService,
        AppSettings appSettings)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
    }

    public abstract Task<CopyCustomerResult> Copy(CopyCustomerRequest request);
}