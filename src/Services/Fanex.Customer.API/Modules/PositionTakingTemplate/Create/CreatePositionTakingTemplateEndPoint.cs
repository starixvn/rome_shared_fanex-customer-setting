using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePositionTakingTemplateEndPoint : IEndpoint<IResult, CreatePositionTakingTemplateRequest, IDynamicRepository>
{
    private readonly AppSettings appSettings;

    public CreatePositionTakingTemplateEndPoint(IOptions<AppSettings> appSettings)
    {
        this.appSettings = appSettings.Value;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/pt-templates", (CreatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
            => HandleAsync(request, dynamicRepository))
            .Produces<Result>()
            .WithTags("PositionTakingTemplate");
    }

    public async Task<IResult> HandleAsync(CreatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
    {
        var command = new CreatePositionTakingTemplateCommand(
            request.CustId,
            request.TemplateName,
            request.Settings.BuildXMLSettings(),
            request.IsDefault,
            appSettings.CreationStatus,
            request.Language,
            request.IP);

        await dynamicRepository.ExecuteAsync(command);

        return Results.Ok(new Result(command.ErrorCode, command.ErrMsg));
    }
}