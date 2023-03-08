using Fanex.Customer.API.Modules.Customer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCreditInfoEndpoint : IEndpoint<IResult, int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/credit", (int custId, ICustomerService customerService)
            => HandleAsync(custId, customerService))
            .Produces<CreditInfo>();
    }

    public async Task<IResult> HandleAsync(int custId, ICustomerService customerService)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await customerService.GetCreditInfo(custId).ConfigureAwait(false));
    }
}