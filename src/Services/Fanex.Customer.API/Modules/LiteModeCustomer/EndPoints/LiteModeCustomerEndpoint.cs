using Fanex.Customer.API.Modules.LiteModeCustomer.Services;
using Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.LiteModeCustomer.EndPoints;

public class LiteModeCustomerEndpoint : IEndpoint<IResult, int, ILiteModeCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/litemode/query/{custId}", (int custId, ILiteModeCustomerService customerService)
            => HandleAsync(custId, customerService))
            .Produces<LiteModeCustomerSetting>();
    }

    public async Task<IResult> HandleAsync(int custId, ILiteModeCustomerService customerService)
    {
        return Results.Ok(customerService.Query(custId));

    }
}