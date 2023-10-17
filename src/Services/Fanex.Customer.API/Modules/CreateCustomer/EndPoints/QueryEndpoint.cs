using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class QueryEndpoint : IEndpoint<IResult, QueryRequest, IQueryCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/query", (QueryRequest request, IQueryCustomerService service)
            => HandleAsync(request, service))
            .Produces<CustomerSetting>().WithTags("Create Customer");
    }

    public async Task<IResult> HandleAsync(QueryRequest request, IQueryCustomerService service)
    {
        return Results.Ok(await service.Query(request));
    }
}