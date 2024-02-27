using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class ProtectionCodeEndpoint : 
    IEndpoint<string, int, int, IProtectionCodeService>,
    IEndpoint<Result, UpdateProtectionCodeParam, IProtectionCodeService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        var enpoint = app.MapGroup("/api/customer/protection-code").WithTags(EnpointTags.CustomerSettings);

        enpoint.MapGet("/", (int custId, int subAccId, IProtectionCodeService protectionCodeService)
            => HandleAsync(custId, subAccId, protectionCodeService))
            .Produces<string>();

        enpoint.MapPost("/", (UpdateProtectionCodeParam param, IProtectionCodeService protectionCodeService)
            => HandleAsync(param, protectionCodeService))
            .Produces<Result>();
    }

    public async Task<string> HandleAsync(int custId, int subAccId, IProtectionCodeService protectionCodeService)
    {
        return await protectionCodeService.GetAsync(custId, subAccId);
    }

    public async Task<Result> HandleAsync(UpdateProtectionCodeParam request1, IProtectionCodeService request2)
    {
        return await request2.UpdateAsync(request1);
    }
}