using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetSuggestCashMemberUsernameEndpoint : IEndpoint<IResult, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{UplineUsername}/suggest-cash-member-username", (string uplineUserName, ICustomerService customerService)
            => HandleAsync(uplineUserName, customerService))
            .Produces<string>()
            .WithTags("Cash Service");
    }

    public async Task<IResult> HandleAsync(string uplineUserName, ICustomerService customerService)
    {
        if (!string.IsNullOrWhiteSpace(uplineUserName))
        {
            var result = await customerService.GetSuggestCashMemberUsername(uplineUserName);
            return Results.Ok(result.CustomData);
        }

        return Results.BadRequest();
    }
}