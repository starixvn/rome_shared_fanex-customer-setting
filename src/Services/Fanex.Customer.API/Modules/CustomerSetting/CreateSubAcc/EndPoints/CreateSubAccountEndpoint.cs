using Fanex.Customer.API.Modules.CreateSubAcc.Requests;
using Fanex.Customer.API.Modules.CreateSubAcc.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateSubAcc.EndPoints;

public class CreateSubAccountEndpoint : IEndpoint<IResult, CreateSubAccountRequest, ICreateSubAccountService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/subaccount", (CreateSubAccountRequest request, ICreateSubAccountService service)
            => HandleAsync(request, service))
            .Produces<CustomerSettingResult>().WithTags("Customer Settings -> Sub Account");
    }

    public async Task<IResult> HandleAsync(CreateSubAccountRequest request, ICreateSubAccountService service)
    {
        if (!service.ValidateRequest(request))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.Create(request));
    }
}