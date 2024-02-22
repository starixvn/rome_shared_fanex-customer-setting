using Fanex.Customer.API.Modules.CopyCustomer.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CopySingleCustomerEndpoint : IEndpoint<IResult, CopyCustomerRequest, ICopyCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/copy-single", (CopyCustomerRequest request, ICopyCustomerService copyCustomerService)
           => HandleAsync(request, copyCustomerService))
            .Produces<CopyMultipleCustomerResult>()
            .WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(CopyCustomerRequest request, ICopyCustomerService copyCustomerService)
    {
        if (request.CustLevel == 4)
        {
            await Logging.LogService.InfoAsync(System.Text.Json.JsonSerializer.Serialize(request));
        }
        
        if (request.IsValid())
        {
            return Results.Ok(await copyCustomerService.CopyCustomer(request));
        }

        return Results.BadRequest();
    }
}