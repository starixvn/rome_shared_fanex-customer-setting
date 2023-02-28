namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public interface IThirdPartyProductService
{
    Task SyncMasterOrAgentSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param);

    Task SyncMemberSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param);
}

public class ThirdPartyProductService : IThirdPartyProductService
{
    private readonly ING2Service ng2Service;
    private readonly IDCSService dcsService;
    private readonly IKenoService kenoService;

    public ThirdPartyProductService(
        ING2Service ng2Service,
        IDCSService dcsService,
        IKenoService kenoService)
    {
        this.ng2Service = ng2Service;
        this.dcsService = dcsService;
        this.kenoService = kenoService;
    }

    public async Task SyncMasterOrAgentSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param)
    {
        await ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param);
    }

    public async Task SyncMemberSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param)
    {
        await dcsService.InsertMemberSettings(
            createdCustomer.CustId,
            sourceCustomer.CustId,
            actor.CustId,
            param.ActorSubAccId,
            param.IP);

        kenoService.UpdateMemberSettings(
            createdCustomer.CustId,
            sourceCustomer.CustId,
            param);

        await ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param);
    }
}

