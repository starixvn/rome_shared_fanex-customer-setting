using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.SearchCustomer;

public class SearchCustomerEndpoint : IEndpoint<IResult, SearchCustomerRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/search", (SearchCustomerRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(SearchCustomerRequest request, ICustomerService customerService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await customerService.SearchCustomer(request));
        }

        return Results.BadRequest();

    }
}