using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class CreateEndpoint : IEndpoint<IResult, CreateRequest, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer", (CreateRequest request, ICreateCustomerService service)
            => HandleAsync(request, service))
            .Produces<CustomerSettingResult>().WithTags("Customer Settings");
       
    }

    public async Task<IResult> HandleAsync(CreateRequest request, ICreateCustomerService service)
    {
        if (!service.ValidateRequest(request))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.Create(request));
    }
}