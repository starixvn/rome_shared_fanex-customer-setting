using Fanex.Customer.API.Modules.CopyCustomer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CopyMultipleCustomersEndpoint : IEndpoint<IResult, CopyMultipleCustomersRequest, ICopyCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/copy-multiple", (CopyMultipleCustomersRequest request, ICopyCustomerService copyCustomerService)
            => HandleAsync(request, copyCustomerService))
            .Produces<CopyMultipleCustomerResult>().WithTags(EndpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(CopyMultipleCustomersRequest request, ICopyCustomerService copyCustomerService)
    {
        if (request.CustLevel == 4)
        {
            await Logging.LogService.InfoAsync(System.Text.Json.JsonSerializer.Serialize(request));
        }

        if (request.IsValid())
        {
            return Results.Ok(await copyCustomerService.CopyCustomers(request));
        }

        return Results.BadRequest();
    }
}