using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class UserNameSuggestionEndpoint : 
    IEndpoint<IResult, UserNameSuggestionQuery, UsernameSuggestionService>,
    IEndpoint<IResult, UsernameSuggestionByRoleQuery, UsernameSuggestionService>,
    IEndpoint<IResult, UpdateUserSearchQuery, UsernameSuggestionService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/username-suggestion", (UserNameSuggestionQuery request1, UsernameSuggestionService request2) => HandleAsync(request1, request2))
            .WithTags(EndpointTags.CustomerInfos)
            .Produces<IResult>();

        app.MapPut("/api/customer/username-suggestion", (UpdateUserSearchQuery request1, UsernameSuggestionService request2) => HandleAsync(request1, request2))
            .WithTags(EndpointTags.CustomerInfos)
            .Produces<IResult>();

        app.MapPost("/api/customer/username-suggestion-by-role", (UsernameSuggestionByRoleQuery request1, UsernameSuggestionService request2) => HandleAsync(request1, request2))
            .WithTags(EndpointTags.CustomerInfos)
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(UserNameSuggestionQuery request1, UsernameSuggestionService request2)
    {
        if (!request1.IsValid())
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.GetUsernamesSuggestion(request1.ActorId, request1.Query, request1.IsInternal, request1.DirectDownlines, request1.IsAccInfo));
    }

    public async Task<IResult> HandleAsync(UsernameSuggestionByRoleQuery request1, UsernameSuggestionService request2)
    {
        if (request1.CustId <= 0 || request1.RoleId <= 0 || request1.RoleId >= 4 || string.IsNullOrWhiteSpace(request1.Username))
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.GetUsernameSuggestionByRole(request1.CustId, request1.Username, request1.RoleId));
    }

    public async Task<IResult> HandleAsync(UpdateUserSearchQuery request1, UsernameSuggestionService request2)
    {
        if (!request1.IsValid())
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.UpdateUserSearch(request1.UserId, request1.Search, request1.FirstName, request1.LastName, request1.Nickname));
    }
}