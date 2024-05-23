using Fanex.Customer.API.Modules.Customer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetDownlineByUsernameEndpoint : IEndpoint<IResult, int, int, string>
{
    private readonly ICustomerService customerService;

    public GetDownlineByUsernameEndpoint(ICustomerService customerService)
    {
        this.customerService = customerService;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/{subAccountId}/downlines/{username}", (int custId, int subAccountId, string username)
            => HandleAsync(custId, subAccountId, username))
            .Produces<Downline>()
            .WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(int custId, int subAccountId, string username)
    {
        if (custId > 0 && !string.IsNullOrWhiteSpace(username))
        {
            return Results.Ok(await customerService.GetDownlineByUsername(custId, subAccountId, username).ConfigureAwait(false));
        }

        return Results.BadRequest();
    }
}