using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Http.Features;
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
