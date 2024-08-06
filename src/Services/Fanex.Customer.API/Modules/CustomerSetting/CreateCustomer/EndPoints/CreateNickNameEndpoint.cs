using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class CreateNickNameEndpoint : IEndpoint<IResult, CreateNickNameRequest, ICreateNickNameService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/nickname", (CreateNickNameRequest request, ICreateNickNameService service)
            => HandleAsync(request, service))
            .Produces<CustomerSettingResult>().WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(CreateNickNameRequest request, ICreateNickNameService service)
    {
        if (!service.ValidateRequest(request.NickName))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("NicknameRule", request.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.Create(request));
    }
}