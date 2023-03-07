using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetMaxCreditEndpoint : IEndpoint<IResult, int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/max-credit", (int custId, ICustomerService customerService)
            => HandleAsync(custId, customerService))
            .Produces<string>();
    }

    public async Task<IResult> HandleAsync(int custId, ICustomerService customerService)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await customerService.GetMaxCredit(custId).ConfigureAwait(false));
    }
}