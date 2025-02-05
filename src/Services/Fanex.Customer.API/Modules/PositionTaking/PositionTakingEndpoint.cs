using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTaking;

public class PositionTakingEndpoint : IEndpoint<IResult, PositionTakingRequest, IPositionTakingService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/position-taking", ([AsParameters] PositionTakingRequest request, IPositionTakingService service)
            => HandleAsync(request, service))
            .Produces<IResult>().WithTags(EndpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(PositionTakingRequest request, IPositionTakingService service)
    {
        if (!request.IsValid())
        {
            return Results.BadRequest();    
        }

        var result = await service.GetPT(request);
        return Results.Ok(result);
    }
}
