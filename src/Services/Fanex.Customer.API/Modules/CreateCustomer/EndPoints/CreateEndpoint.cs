using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Extensions;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class CreateEndpoint : IEndpoint<IResult, CreateRequest, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/create", (CreateRequest request, ICreateCustomerService service)
            => HandleAsync(request, service))
            .Produces<CreateCustomerResult>().WithTags("Create Customer");
    }

    public async Task<IResult> HandleAsync(CreateRequest request, ICreateCustomerService service)
    {
        //Fanex.Logging.LogService.Info(JsonConvert.SerializeObject(request));
        var generalInfo = request.Settings.GeneralInfo;
        if ((!string.IsNullOrWhiteSpace(generalInfo.FirstName) && generalInfo.FirstName.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(generalInfo.LastName) && generalInfo.LastName.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(generalInfo.Phone) && generalInfo.Phone.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(generalInfo.MobilePhone) && generalInfo.MobilePhone.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(generalInfo.MobilePhone) && generalInfo.MobilePhone.ContainsHTML())
        || !service.ValidateRequest(request))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.Create(request));
    }
}