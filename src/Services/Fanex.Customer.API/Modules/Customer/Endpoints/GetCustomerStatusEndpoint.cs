using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCustomerStatusEndpoint : IEndpoint<IResult, int, int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/{subAccId}/status", (int custId, int subAccId, ICustomerService customerService)
          => HandleAsync(custId, subAccId, customerService))
          .Produces<IResult>()
          .WithTags(EndpointTags.CustomerSettings);

    }

    public async Task<IResult> HandleAsync(int custId, int subAccId, ICustomerService customerService)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await customerService.GetCustomerStatus(custId, subAccId));
    }
}