using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, int, int, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/{roleId}/{language}/queryPTTemplate", (int custId, int roleId, string language, ICreateCustomerService customerService)
            => HandleAsync(custId, roleId, customerService))
            .Produces<CustomerSetting>().WithTags("LiteMode");
    }

    public async Task<IResult> HandleAsync(int custId, int roleId, ICreateCustomerService customerService)
    {
        return Results.Ok(await customerService.QueryPTTemplate(custId, roleId));
    }
}