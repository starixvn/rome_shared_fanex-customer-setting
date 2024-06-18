using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.EditCustomer.Services;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.EditCustomer.EndPoints;

public class EditEndpoint : IEndpoint<IResult, EditRequest, IEditCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customer", (EditRequest request, IEditCustomerService service)
           => HandleAsync(request, service))
           .Produces<UpdateGroupTypeResult>().WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(EditRequest request, IEditCustomerService service)
    {
        if (!service.ValidateRequest(request))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.Update(request));
    }
}

public class EditMultipleEndpoint : IEndpoint<IResult, EditMultipleRequest, IEditCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customers", (EditMultipleRequest request, IEditCustomerService service)
           => HandleAsync(request, service))
           .Produces<List<UpdateCustomerResult>>().WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(EditMultipleRequest request, IEditCustomerService service)
    {
        if (!service.ValidateMultipleRequest(request))
        {
            return Results.Ok(Shared.Models.Result.CreateErrorResult(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        }

        return Results.Ok(await service.UpdateMultiple(request));
    }
}