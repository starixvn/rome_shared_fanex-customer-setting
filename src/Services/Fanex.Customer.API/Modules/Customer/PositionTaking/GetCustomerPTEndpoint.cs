using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.PositionTaking;

public class GetCustomerPTEndpoint : IEndpoint<IResult, GetCustomerPTRequest, CustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/pt", (GetCustomerPTRequest request, CustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(GetCustomerPTRequest request, CustomerService customerService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await customerService.GetCustomerPT(request));
        }

        return Results.BadRequest();
    }
}