using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CheckLimitNewDownlineEndpoint : IEndpoint<IResult, CheckAllLimitedRulesRequest, ICopyCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/CheckLimitNewDownline", (CheckAllLimitedRulesRequest request, ICopyCustomerService copyCustomerService)
            => HandleAsync(request, copyCustomerService))
            .Produces<Result>();
    }

    public async Task<IResult> HandleAsync(CheckAllLimitedRulesRequest request, ICopyCustomerService copyCustomerService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await copyCustomerService.CheckLimitNewDownline(request).ConfigureAwait(false));
        }

        return Results.BadRequest();
    }
}