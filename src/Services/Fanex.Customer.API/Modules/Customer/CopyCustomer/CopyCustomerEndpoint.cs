using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class CopyCustomerEndPoint : IEndpoint<IResult, CopyCustomerRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/copy", (CopyCustomerRequest request)
            => HandleAsync(request))
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(CopyCustomerRequest request)
    {
        if (request.IsValid())
        {
            return Results.Ok();
        }

        return Results.BadRequest();
    }
}