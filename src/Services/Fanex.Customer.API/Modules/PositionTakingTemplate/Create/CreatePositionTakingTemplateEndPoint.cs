using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePositionTakingTemplateEndPoint : IEndpoint<IResult, CreatePositionTakingTemplateRequest, IPositionTakingTemplateService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/pt-templates", (CreatePositionTakingTemplateRequest request, IPositionTakingTemplateService positionTakingTemplateService)
            => HandleAsync(request, positionTakingTemplateService))
            .Produces<Result>()
            .WithTags("PositionTakingTemplate");
    }

    public async Task<IResult> HandleAsync(CreatePositionTakingTemplateRequest request, IPositionTakingTemplateService positionTakingTemplateService)
    {
        if (request.IsValid())
        {
            var response = await positionTakingTemplateService.Create(request);
            return Results.Ok(response);
        }

        return Results.BadRequest();
    }
}