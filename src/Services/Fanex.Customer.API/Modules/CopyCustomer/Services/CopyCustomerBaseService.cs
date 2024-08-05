using Fanex.Customer.API;
using Fanex.Customer.API.Modules.CopyCustomer;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;

public abstract class CopyCustomerBaseService
{
    protected readonly IScaleoutDBService scaleoutDBService;

    protected readonly IMessageService messageService;

    protected readonly AppSettings appSettings;

    public CopyCustomerBaseService(IMessageService messageService,
        AppSettings appSettings,
        IScaleoutDBService scaleoutDBService)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
        this.scaleoutDBService = scaleoutDBService;
    }

    public abstract Task<CopyCustomerResult> Copy(CopyCustomerRequest request);
}