using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class DeletePositionTakingTemplateEndpoint : IEndpoint<IResult, DeletePositionTakingTemplateRequest, IDynamicRepository>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customer/pt-templates/delete", (DeletePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
            => HandleAsync(request, dynamicRepository))
            .Produces<Result>()
            .WithTags("PositionTakingTemplate");
    }

    public async Task<IResult> HandleAsync(DeletePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
    {
        var command = new DeletePositionTakingTemplateCommand(request.CustId, request.TemplateId, request.Language, request.IP);
        await dynamicRepository.ExecuteAsync(command);
        return Results.Ok(new Result(command.ErrorCode, command.ErrMsg));
    }
}