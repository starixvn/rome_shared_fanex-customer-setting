using Fanex.Customer.API.Modules.CreateSubAcc.Requests;
using Fanex.Customer.API.Modules.CreateSubAcc.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateSubAcc.EndPoints;

public class CreateNickNameEndpoint : IEndpoint<IResult, CreateNickNameRequest, ICreateNickNameSubAccountService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/nick-subaccount", (CreateNickNameRequest request, ICreateNickNameSubAccountService service)
            => HandleAsync(request, service))
            .Produces<CustomerSettingResult>().WithTags("Customer Settings -> Sub Account");
    }

    public async Task<IResult> HandleAsync(CreateNickNameRequest request, ICreateNickNameSubAccountService service)
    {
        Console.WriteLine("nickname: " + request.NickName);
        if (!service.ValidateRequest(request))
        {
            return Results.Ok(Shared.Models.Result.Create(StaticResources.Get("NicknameRule", request.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.Create(request));
    }
}