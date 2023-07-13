using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class UpdatePositionTakingTemplateEndpoint : IEndpoint<IResult, UpdatePositionTakingTemplateRequest, IDynamicRepository>
{
    private readonly AppSettings appSettings;
    private readonly IMessageService messageService;

    public UpdatePositionTakingTemplateEndpoint(IOptions<AppSettings> appSettings, IMessageService messageService)
    {
        this.appSettings = appSettings.Value;
        this.messageService = messageService;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customer/pt-templates", (UpdatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
            => HandleAsync(request, dynamicRepository))
            .Produces<PTTemplateResult>()
            .WithTags("PositionTakingTemplate");
    }

    public async Task<IResult> HandleAsync(UpdatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
    {
        var command = new UpdatePositionTakingTemplateCommand(
            request.CustId,
            request.TemplateId,
            request.Settings.BuildXMLSettings(),
            request.IsDefault,
            request.SubAccId,
            appSettings.CreationStatus,
            LanguageUtils.ToLanguageKey(request.Language),
            request.IP);
        await dynamicRepository.ExecuteAsync(command);
        var message = messageService.TranslateMessage(command.ErrType, command.ErrMsg, request.Language);

        return Results.Ok(new PTTemplateResult(command.ErrCode, message.MainMessage, message.SubMessages));
    }
}