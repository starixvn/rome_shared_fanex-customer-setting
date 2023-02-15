using Alpha.CloneAccount.Models;
using Alpha.CloneAccount.Params;

namespace Alpha.CloneAccount.Services;

public interface ThirdPartyProductService
{
    Task SyncMasterOrAgentSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerParam param);

    Task SyncMemberSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerParam param);
}

public class ThirdPartyProductServiceImpl : ThirdPartyProductService
{
    private readonly NG2Service ng2Service;
    private readonly DCSService dcsService;
    private readonly KenoService kenoService;

    public ThirdPartyProductServiceImpl(
        NG2Service ng2Service,
        DCSService dcsService,
        KenoService kenoService)
    {
        this.ng2Service = ng2Service;
        this.dcsService = dcsService;
        this.kenoService = kenoService;
    }

    public async Task SyncMasterOrAgentSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerParam param)
    {
        await ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param);
    }

    public async Task SyncMemberSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerParam param)
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

