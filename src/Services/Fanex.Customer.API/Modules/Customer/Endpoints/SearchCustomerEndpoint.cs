using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class SearchCustomerEndpoint : IEndpoint<IResult, SearchCustomerRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/search", (SearchCustomerRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<SuggestCustomer>();
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