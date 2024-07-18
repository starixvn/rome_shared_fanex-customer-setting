using Fanex.Customer.API.Modules;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class UserNameSuggestionEndpoint : IEndpoint<IResult, UserNameSuggestionQuery, UsernameSuggestionService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/username-suggestion", ([AsParameters] UserNameSuggestionQuery request1, UsernameSuggestionService request2) => HandleAsync(request1, request2))
            .WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(UserNameSuggestionQuery request1, UsernameSuggestionService request2)
    {
        if (!request1.IsValid())
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.GetUsernamesSuggestion(request1.ActorId, request1.Query, request1.IsInternal, request1.DirectDownlines, request1.IsAccInfo));
    }
}
