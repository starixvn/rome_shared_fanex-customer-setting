using Fanex.Customer.API.Modules.Customer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCashServiceTotalDownlineEndpoint : IEndpoint<IResult, int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/cash-service-total-downline", (int custId, ICustomerService customerService)
                => HandleAsync(custId, customerService))
            .Produces<TotalCashMember>()
            .WithTags("Cash Service");
    }

    public async Task<IResult> HandleAsync(int custId, ICustomerService customerService)
    {
        if (custId > 0)
        {
            var result = await customerService.GetCashServiceTotalDownline(custId);
            return Results.Ok(result);
        }

        return Results.BadRequest();
    }
}
