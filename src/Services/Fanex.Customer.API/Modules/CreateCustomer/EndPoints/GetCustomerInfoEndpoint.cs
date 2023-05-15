using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetCustomerInfoEndpoint : IEndpoint<IResult, int, int, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/query", (int uplineCustId, int uplineRoleId, string language, ICreateCustomerService customerService)
            => HandleAsync(uplineCustId, uplineRoleId, customerService))
            .Produces<CustomerSetting>().WithTags("LiteMode");
    }

    public async Task<IResult> HandleAsync(int uplineCustId, int uplineRoleId, ICreateCustomerService customerService)
    {
        return Results.Ok(await customerService.Query(uplineCustId, uplineRoleId));
    }
}