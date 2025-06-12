using System.Text.Json;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class CreateCashCustomerEndpoint : IEndpoint<IResult, CreateRequest, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cash-customer", (CreateRequest request, ICreateCustomerService service)
            => HandleAsync(request, service))
            .Produces<CreateCustomerResult>().WithTags(EndpointTags.CashService);
    }

    public async Task<IResult> HandleAsync(CreateRequest request, ICreateCustomerService service)
    {
        if (!service.ValidateRequest(request))
        {
            return Results.Ok(Shared.Models.Result.CreateError(StaticResources.Get("InvalidParameterValue", request.Context.Language ?? LanguageUtils.DefaultLanguage)));
        }
        
        request.Context.IsCashService = true;
        request.Context.NoOfCreateAccount = 1; // Only create 1 account at a time for cash service
        return Results.Ok(await service.Create(request));
    }
}