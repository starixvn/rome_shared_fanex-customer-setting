using Fanex.Customer.API.Modules.CopyCustomer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CopyMultipleCustomersEndpoint : IEndpoint<IResult, CopyMultipleCustomersRequest, ICopyCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/copy-multiple", (CopyMultipleCustomersRequest request, ICopyCustomerService copyCustomerService)
            => HandleAsync(request, copyCustomerService))
            .Produces<CopyMultipleCustomerResult>();
    }

    public async Task<IResult> HandleAsync(CopyMultipleCustomersRequest request, ICopyCustomerService copyCustomerService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await copyCustomerService.CopyCustomers(request));
        }

        return Results.BadRequest();
    }
}