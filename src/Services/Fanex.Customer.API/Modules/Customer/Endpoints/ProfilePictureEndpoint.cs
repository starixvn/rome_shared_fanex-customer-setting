using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class ProfilePictureEndpoint : 
    IEndpoint<ActorProfilePictureModel, int, IProfilePictureService>,
    IEndpoint<Result, UpdateActorProfilePictureParam, IProfilePictureService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        var enpoint = app.MapGroup("/api/customer/profile-picture").WithTags(EndpointTags.CustomerSettings);

        enpoint.MapGet("/", (int custId, IProfilePictureService profilePictureService)
            => HandleAsync(custId, profilePictureService))
            .Produces<ActorProfilePictureModel>();

        enpoint.MapPost("/", (UpdateActorProfilePictureParam param, IProfilePictureService profilePictureService)
            => HandleAsync(param, profilePictureService))
            .Produces<Result>();
    }

    public async Task<ActorProfilePictureModel> HandleAsync(int request, IProfilePictureService service)
    {
        return await service.GetActorProfilePicture(request);
    }

    public async Task<Result> HandleAsync(UpdateActorProfilePictureParam request, IProfilePictureService service)
    {
        return await service.UpdateActorProfilePicture(request);
    }
}