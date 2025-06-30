using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCurrentWinAmountEndpoint : IEndpoint<IResult, GetCurrentWinAmountRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/current-win-amount", ([AsParameters] GetCurrentWinAmountRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<CustomerInfo>()
            .WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(GetCurrentWinAmountRequest request, ICustomerService customerService)
    {
        if (request.CustId > 0)
        {
            return Results.Ok(await customerService.GetCurrentWinAmount(request));
        }

        return Results.BadRequest();
    }
}
