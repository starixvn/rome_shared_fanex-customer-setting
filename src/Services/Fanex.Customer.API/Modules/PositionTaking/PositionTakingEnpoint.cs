using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTaking;

public class PositionTakingEnpoint : IEndpoint<IResult, PositionTakingRequest, IPositionTakingService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> HandleAsync(PositionTakingRequest request1, IPositionTakingService request2)
    {
        throw new NotImplementedException();
    }
}
