using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetSuggestDirectMemberUsernameEndpoint : IEndpoint<IResult, GetSuggestDirectMemberUsernameRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{UplineUsername}/suggest-direct-member-username", ([AsParameters] GetSuggestDirectMemberUsernameRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<string>();
    }

    public async Task<IResult> HandleAsync(GetSuggestDirectMemberUsernameRequest request, ICustomerService customerService)
    {
        if (request.IsValid())
        {
            var result = await customerService.GetSuggestDirectMemberUsername(request).ConfigureAwait(false);
            return Results.Ok(result.CustomData);
        }

        return Results.BadRequest();
    }
}