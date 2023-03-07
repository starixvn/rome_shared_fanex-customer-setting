using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetSuggestUsernameEndpoint : IEndpoint<IResult, GetSuggestUsernameRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/suggest-username", (GetSuggestUsernameRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<string>();
    }

    public async Task<IResult> HandleAsync(GetSuggestUsernameRequest request, ICustomerService customerService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await customerService.GetSuggestUsername(request).ConfigureAwait(false));
        }

        return Results.BadRequest();
    }
}