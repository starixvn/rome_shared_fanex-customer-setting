using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePositionTakingTemplateEndPoint : IEndpoint<IResult, CreatePositionTakingTemplateRequest, IDynamicRepository>
{
    private readonly AppSettings appSettings;
    private readonly IMessageService messageService;

    public CreatePositionTakingTemplateEndPoint(IOptions<AppSettings> appSettings, IMessageService messageService)
    {
        this.appSettings = appSettings.Value;
        this.messageService = messageService;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/pt-templates", (CreatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
            => HandleAsync(request, dynamicRepository))
            .Produces<PTTemplateResult>()
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
        var message = messageService.TranslateMessage(command.ErrType, command.ErrMsg, SettingLevel.SportTypeBetType, request.Language);

        return Results.Ok(new PTTemplateResult(command.TemplateId, command.ErrCode, message.MainMessage, message.SubMessages));
    }
}