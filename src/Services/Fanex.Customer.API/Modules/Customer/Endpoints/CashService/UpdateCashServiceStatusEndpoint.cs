using MinimalApi.Endpoint;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class UpdateCashServiceStatusEndpoint : IEndpoint<Result, UpdateCashServiceStatusRequest, ICashCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/cash-service-status", async (UpdateCashServiceStatusRequest request, ICashCustomerService customerService)
            => await HandleAsync(request, customerService))
            .Produces<Result>()
            .WithTags("Cash Service");
    }

    public async Task<Result> HandleAsync(UpdateCashServiceStatusRequest request, ICashCustomerService customerService)
        => await customerService.UpdateEnableCashService(request);
}
