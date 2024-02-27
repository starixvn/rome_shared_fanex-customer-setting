using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Extensions;
using Fanex.Customer.API.Shared.Utils;
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
            .WithTags(EnpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(CreatePositionTakingTemplateRequest request, IDynamicRepository dynamicRepository)
    {
        if (request.TemplateName.ContainsHTML())
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Language ?? LanguageUtils.DefaultLanguage)));
        }

        var command = new CreatePositionTakingTemplateCommand(
            request.CustId,
            request.TemplateName,
            request.Settings.BuildXMLSettings(),
            request.IsDefault,
            request.SubAccId,
            appSettings.SettingStatus,
            LanguageUtils.ToLanguageKey(request.Language),
            request.IP);

        await dynamicRepository.ExecuteAsync(command);
        var message = messageService.TranslateMessage(command.MsgType, command.ErrMsg, request.Language);

        return Results.Ok(new CreatePTTemplateResult(command.TemplateId, command.ErrCode, message.MainMessage, message.SubMessages));
    }
}