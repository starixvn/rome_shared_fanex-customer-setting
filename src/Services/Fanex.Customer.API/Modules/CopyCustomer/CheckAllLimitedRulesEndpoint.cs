using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CheckAllLimitedRulesEndpoint : IEndpoint<IResult, CheckAllLimitedRulesRequest, ICopyCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/CheckAllLimitedRules", (CheckAllLimitedRulesRequest request, ICopyCustomerService copyCustomerService)
            => HandleAsync(request, copyCustomerService))
            .Produces<Result>();
    }

    public async Task<IResult> HandleAsync(CheckAllLimitedRulesRequest request, ICopyCustomerService copyCustomerService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await copyCustomerService.CheckAllLimitedRules(request).ConfigureAwait(false));
        }

        return Results.BadRequest();
    }
}