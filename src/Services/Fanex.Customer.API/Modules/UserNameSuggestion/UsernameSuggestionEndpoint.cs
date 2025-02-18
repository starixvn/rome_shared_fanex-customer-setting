using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class UserNameSuggestionEndpoint :
    IEndpoint<IResult, UserNameSuggestionQuery, UsernameSuggestionService>,
    IEndpoint<IResult, UsernameSuggestionByRoleQuery, UsernameSuggestionService>,
    IEndpoint<IResult, UpdateUserSearchQuery, UsernameSuggestionService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/username-suggestion", (UserNameSuggestionQuery request, UsernameSuggestionService service) => HandleAsync(request, service))
            .WithTags(EndpointTags.CustomerInfos)
            .Produces<IResult>();

        app.MapPut("/api/customer/username-suggestion", (UpdateUserSearchQuery request, UsernameSuggestionService service) => HandleAsync(request, service))
            .WithTags(EndpointTags.CustomerInfos)
            .Produces<IResult>();

        app.MapPost("/api/customer/username-suggestion-by-role", (UsernameSuggestionByRoleQuery request, UsernameSuggestionService service) => HandleAsync(request, service))
            .WithTags(EndpointTags.CustomerInfos)
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(UserNameSuggestionQuery request, UsernameSuggestionService service)
    {
        if (!request.IsValid())
        {
            return Results.BadRequest();
        }

        return Results.Ok(await service.GetUsernamesSuggestion(request.ActorId, request.Query, request.IsInternal, request.DirectDownlines, request.IsAccInfo, request.ExcludeMember));
    }

    public async Task<IResult> HandleAsync(UsernameSuggestionByRoleQuery request, UsernameSuggestionService service)
    {
        if (request.CustId <= 0 || request.RoleId <= 0 || request.RoleId >= 4 || string.IsNullOrWhiteSpace(request.Username))
        {
            return Results.BadRequest();
        }

        return Results.Ok(await service.GetUsernameSuggestionByRole(request.CustId, request.Username, request.RoleId));
    }

    public async Task<IResult> HandleAsync(UpdateUserSearchQuery request, UsernameSuggestionService service)
    {
        if (!request.IsValid())
        {
            return Results.BadRequest();
        }

        return Results.Ok(await service.UpdateUserSearch(request.UserId, request.Search, request.FirstName, request.LastName, request.Nickname));
    }
}