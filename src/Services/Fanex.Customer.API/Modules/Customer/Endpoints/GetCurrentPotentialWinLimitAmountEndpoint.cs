using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCurrentPotentialWinLimitAmountEndpoint : IEndpoint<IResult, GetCurrentWinLossAmountRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/current-potential-winlimit-amount", ([AsParameters] GetCurrentWinLossAmountRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<CustomerInfo>()
            .WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(GetCurrentWinLossAmountRequest request, ICustomerService customerService)
    {
        if (request.CustId > 0)
        {
            return Results.Ok(await customerService.GetCurrentPotentialWinLimitAmount(request));
        }

        return Results.BadRequest();
    }
}
