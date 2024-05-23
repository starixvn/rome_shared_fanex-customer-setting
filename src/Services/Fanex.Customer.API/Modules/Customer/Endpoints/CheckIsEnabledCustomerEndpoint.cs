using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class CheckIsEnabledCustomerEndpoint : IEndpoint<IResult, int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/IsEnabled", (int custId, ICustomerService customerService)
           => HandleAsync(custId, customerService))
           .Produces<bool>()
           .WithTags(EndpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(int custId, ICustomerService customerService)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await customerService.IsEnableCustomer(custId));
    }
}