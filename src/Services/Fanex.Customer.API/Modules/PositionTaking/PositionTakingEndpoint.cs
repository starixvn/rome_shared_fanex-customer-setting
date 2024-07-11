using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTaking;

public class PositionTakingEndpoint : IEndpoint<IResult, PositionTakingRequest, IPositionTakingService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/position-taking", (PositionTakingRequest request1, IPositionTakingService request2)
            => HandleAsync(request1, request2))
            .Produces<IResult>().WithTags(EndpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(PositionTakingRequest request1, IPositionTakingService request2)
    {
        if (!request1.IsValid())
        {
            return Results.BadRequest();    
        }

        var result = await request2.GetPT(request1);
        return Results.Ok(result);
    }
}
