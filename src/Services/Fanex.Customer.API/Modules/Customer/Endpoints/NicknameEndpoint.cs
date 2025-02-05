using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class NicknameEndpoint : IEndpoint<Result, CreateNicknameParam, INicknameService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/nickname", (CreateNicknameParam request, INicknameService nicknameService)
            => HandleAsync(request, nicknameService))
            .Produces<Result>()
            .WithTags(EndpointTags.CustomerSettings);
    }

    public async Task<Result> HandleAsync(CreateNicknameParam request, INicknameService service)
    {
        return await service.CreateNickname(request, nickNameLoginOnly: true);
    }
}