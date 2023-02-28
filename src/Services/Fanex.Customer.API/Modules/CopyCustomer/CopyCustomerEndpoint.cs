using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CopyCustomerEndPoint : IEndpoint<IResult, CopyCustomerRequest, ICopyCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/copy", (CopyCustomerRequest request, ICopyCustomerService copyCustomerService)
            => HandleAsync(request, copyCustomerService))
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(CopyCustomerRequest request, ICopyCustomerService copyCustomerService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await copyCustomerService.CopyCustomer(request));
        }

        return Results.BadRequest();
    }
}