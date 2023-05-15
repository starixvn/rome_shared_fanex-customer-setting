using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, int, string, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/{language}/queryPTTemplate", (int custId, string language, ICreateCustomerService customerService)
            => HandleAsync(custId, language, customerService))
            .WithTags("LiteMode");
    }

    public async Task<IResult> HandleAsync(int custId, string language, ICreateCustomerService customerService)
    {
        return Results.Ok(await customerService.GetPTRange(custId, language));
    }
}