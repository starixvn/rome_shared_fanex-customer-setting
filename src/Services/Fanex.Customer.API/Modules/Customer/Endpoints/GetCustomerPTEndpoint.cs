using Fanex.Customer.API.Modules.Customer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCustomerPTEndpoint : IEndpoint<IResult, int, byte, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/{custLevel}/pt", (int custId, byte custLevel, ICustomerService customerService)
            => HandleAsync(custId, custLevel, customerService))
            .Produces<CustomerPT>();
    }

    public async Task<IResult> HandleAsync(int custId, byte custLevel, ICustomerService customerService)
    {
        return Results.Ok(await customerService.GetCustomerPT(custId, custLevel));
    }
}