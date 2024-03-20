using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.EditCustomer.Services;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.EditCustomer.EndPoints;

public class EditEndpoint : IEndpoint<IResult, EditMultipleRequest, IEditCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customer", (EditMultipleRequest request, IEditCustomerService service)
           => HandleAsync(request, service))
           .Produces<UpdateGroupTypeResult>().WithTags("Customer Settings");

        app.MapPut("/api/customers", (EditMultipleRequest request, IEditCustomerService service)
           => HandleMultipleAsync(request, service))
           .Produces<List<UpdateCustomerResult>>().WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(EditMultipleRequest request, IEditCustomerService service)
    {
        Fanex.Logging.LogService.Info(JsonConvert.SerializeObject(request));
        if (!service.ValidateRequest(request.Settings))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.Update(request));
    }

    public async Task<IResult> HandleMultipleAsync(EditMultipleRequest request, IEditCustomerService service)
    {
        Fanex.Logging.LogService.Info(JsonConvert.SerializeObject(request));
        if (!service.ValidateRequest(request.Settings))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.UpdateMultiple(request));
    }
}
