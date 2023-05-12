using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class UpdatePositionTakingTemplateEndpoint : IEndpoint<IResult, UpdatePositionTakingTemplateRequest, IDynamicRepository>
{
    private readonly AppSettings appSettings;

    public UpdatePositionTakingTemplateEndpoint(IOptions<AppSettings> appSettings)
    {
        this.appSettings = appSettings.Value;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customer/pt-templates", (UpdatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
            => HandleAsync(request, dynamicRepository))
            .Produces<UpdatePositionTakingTemplateResult>()
            .WithTags("PositionTakingTemplate");
    }

    public async Task<IResult> HandleAsync(UpdatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
    {
        var command = new UpdatePositionTakingTemplateCommand(
            request.CustId,
            request.TemplateId,
            request.Settings.BuildXMLSettings(),
            request.IsDefault,
            appSettings.CreationStatus,
            request.Language,
            request.IP);
        await dynamicRepository.ExecuteAsync(command);
        return Results.Ok(new UpdatePositionTakingTemplateResult(command.ErrCode, command.ErrType, command.ErrMsg));
    }
}