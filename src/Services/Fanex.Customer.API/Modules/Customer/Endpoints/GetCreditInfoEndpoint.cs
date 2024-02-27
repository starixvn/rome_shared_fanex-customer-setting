using Fanex.Customer.API.Modules.Customer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCreditInfoEndpoint : IEndpoint<IResult, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{username}/credit", (string username, ICustomerService customerService)
            => HandleAsync(username, customerService))
            .Produces<CreditInfo>()
            .WithTags(EnpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(string username, ICustomerService customerService)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Results.BadRequest();
        }

        return Results.Ok(await customerService.GetCreditInfo(username).ConfigureAwait(false));
    }
}