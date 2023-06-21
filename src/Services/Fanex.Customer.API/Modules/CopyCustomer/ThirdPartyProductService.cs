using Fanex.Customer.API.Modules.CopyCustomer.DCS;
using Fanex.Customer.API.Modules.Customer.Models;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public interface IThirdPartyProductService
{
    Task SyncMemberSettings(
        CustomerInfo createdCustomer,
        CustomerInfo sourceCustomer,
        CustomerInfo actor,
        CopyCustomerRequest param,
        byte settingStatus);
}

public class ThirdPartyProductService : IThirdPartyProductService
{
    private readonly IDCSService dcsService;

    public ThirdPartyProductService(
        IDCSService dcsService)
    {
        this.dcsService = dcsService;
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
    }
}

