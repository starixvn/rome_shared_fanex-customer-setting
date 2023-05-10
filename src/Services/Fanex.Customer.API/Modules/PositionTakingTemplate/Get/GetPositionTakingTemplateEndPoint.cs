using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class GetPositionTakingTemplateEndPoint : IEndpoint<IResult, int, IPositionTakingTemplateService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/pt-templates", (int custId, IPositionTakingTemplateService positionTakingTemplateService)
            => HandleAsync(custId, positionTakingTemplateService))
            .Produces<IEnumerable<PTTemplate>>()
            .WithTags("PositionTakingTemplate");
    }

    public async Task<IResult> HandleAsync(int custId, IPositionTakingTemplateService positionTakingTemplateService)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await positionTakingTemplateService.GetByCustId(custId));
    }
}