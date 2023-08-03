using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Rules;

public class CheckAllLimitedRulesEndpoint : IEndpoint<IResult, CheckAllLimitedRulesRequest, IRulesService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rules/CheckAllLimitedRules", (CheckAllLimitedRulesRequest request, IRulesService rulesService)
            => HandleAsync(request, rulesService))
            .Produces<Result>();
    }

    public async Task<IResult> HandleAsync(CheckAllLimitedRulesRequest request, IRulesService rulesService)
    {
        if (request.IsValid())
        {
            return Results.Ok(await rulesService.CheckAllLimitedRules(request).ConfigureAwait(false));
        }

        return Results.BadRequest();
    }
}