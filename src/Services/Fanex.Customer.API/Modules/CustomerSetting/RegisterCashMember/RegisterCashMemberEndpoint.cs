using System.Text.Json;
using Fanex.Customer.API.Modules.CustomerSetting.RegisterCashMember.Criteria;
using Fanex.Customer.API.Modules.CustomerSetting.RegisterCashMember.Requests;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CustomerSetting.RegisterCashMember;

public class RegisterCashMemberEndpoint : IEndpoint<IResult, RegisterCashMemberRequest, IRegisterCashMemberService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cash-member/register", (RegisterCashMemberRequest request, IRegisterCashMemberService service) 
        => HandleAsync(request, service))
        .WithName("RegisterCashMember")
        .WithTags("Cash Service");
    }

    public async Task<IResult> HandleAsync(RegisterCashMemberRequest request, IRegisterCashMemberService service)
    {
        // Fanex.Logging.LogService.Info(JsonSerializer.Serialize(request));
        // if (!service.ValidateRequest(request))
        // {
        //     return Results.Ok(Shared.Models.Result.CreateError(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        // }

        return Results.Ok(await service.RegisterCashMember(new RegisterCashMemberCommand(
            request.AgentId, 
            request.UserName, 
            request.FirstName, 
            request.LastName, 
            request.PhoneNumber)));
    }
}