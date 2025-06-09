using MinimalApi.Endpoint;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCashServiceStatusEndpoint : IEndpoint<int, int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/cash-service-status", (int custId, ICustomerService customerService)
            => HandleAsync(custId, customerService))
            .Produces<int>()
            .WithTags("Cash Service");
    }

    public async Task<int> HandleAsync(int custId, ICustomerService customerService)
    {
        if (custId > 0)
        {
            return await customerService.GetCashServiceStatus(custId);
        }
        return 0;
    }
}
