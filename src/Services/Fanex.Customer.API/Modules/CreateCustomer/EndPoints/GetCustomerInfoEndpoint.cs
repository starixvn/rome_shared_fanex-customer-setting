using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetCustomerInfoEndpoint : IEndpoint<IResult, CreateCustomerRequest, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/query", (CreateCustomerRequest request, ICreateCustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<CustomerSetting>().WithTags("Create Customer");
    }

    public async Task<IResult> HandleAsync(CreateCustomerRequest request, ICreateCustomerService customerService)
    {
        return Results.Ok(await customerService.Query(request));
    }
}