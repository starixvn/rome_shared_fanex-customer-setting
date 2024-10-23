using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCurrentWinLossAmountEndpoint : IEndpoint<IResult, GetCurrentWinLossAmountRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/current-winloss-amount", ([AsParameters] GetCurrentWinLossAmountRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<CustomerInfo>()
            .WithTags("CustomerInfo");
    }

    public async Task<IResult> HandleAsync(GetCurrentWinLossAmountRequest request, ICustomerService customerService)
    {
        if (request.CustId > 0)
        {
            return Results.Ok(await customerService.GetCurrentWinLossAmount(request));
        }

        return Results.BadRequest();
    }
}
