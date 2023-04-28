using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetCustomerInfoEndpoint : IEndpoint<IResult, int, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}", (int custId, ICreateCustomerService customerService)
            => HandleAsync(custId, customerService))
            .Produces<CustomerSetting>();
    }

    public async Task<IResult> HandleAsync(int custId, ICreateCustomerService customerService)
    {
        return Results.Ok(await customerService.Query(custId));

    }
}