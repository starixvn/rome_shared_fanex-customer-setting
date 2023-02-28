using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class CopyCustomerEndPoint : IEndpoint<IResult, CopyCustomerRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/copy", (CopyCustomerRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(CopyCustomerRequest request, ICustomerService customerService)
    {
        if (request.IsValid())
        {
            return Results.Ok();
        }

        return Results.BadRequest();
    }
}