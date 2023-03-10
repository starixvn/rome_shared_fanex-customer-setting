using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class CheckExistedCustomerEndpoint : IEndpoint<IResult, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{username}/exist", (string username, ICustomerService customerService)
            => HandleAsync(username, customerService))
            .Produces<bool>();
    }

    public async Task<IResult> HandleAsync(string username, ICustomerService customerService)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Results.BadRequest();
        }

        return Results.Ok(await customerService.CheckExistedCustomer(username));
    }
}