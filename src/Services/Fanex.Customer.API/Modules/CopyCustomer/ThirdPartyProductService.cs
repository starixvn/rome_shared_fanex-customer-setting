using Fanex.Customer.API.Modules.CopyCustomer.DCS;
using Fanex.Customer.API.Modules.CopyCustomer.NG2;
using Fanex.Customer.API.Modules.Customer.Models;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public interface IThirdPartyProductService
{
    Task SyncMasterOrAgentSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param,
        byte settingStatus);

    Task SyncMemberSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param,
        byte settingStatus);
}

public class ThirdPartyProductService : IThirdPartyProductService
{
    private readonly ING2Service ng2Service;
    private readonly IDCSService dcsService;

    public ThirdPartyProductService(
        ING2Service ng2Service,
        IDCSService dcsService)
    {
        this.ng2Service = ng2Service;
        this.dcsService = dcsService;
    }

    public async Task SyncMasterOrAgentSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param,
        byte settingStatus)
    {
        await ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param, settingStatus);
    }

    public async Task SyncMemberSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param,
        byte settingStatus)
    {
        await dcsService.InsertMemberSettings(
            createdCustomer.CustId,
            sourceCustomer.CustId,
            actor.CustId,
            param.ActorSubAccId,
            param.IP);

        await ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param, settingStatus);
    }
}

