using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCashServiceTotalDownlineEndpoint : IEndpoint<int, int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/cash-service-total-downline", (int custId, ICustomerService customerService)
                => HandleAsync(custId, customerService))
            .Produces<int>()
            .WithTags("Cash Service");
    }

    public async Task<int> HandleAsync(int custId, ICustomerService customerService)
    {
        if (custId > 0)
        {
            return await customerService.GetCashServiceTotalDownline(custId);
        }

        return 0;
    }
}
