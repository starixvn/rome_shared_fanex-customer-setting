using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.QueryCustomer.Requests;
using Fanex.Customer.API.Modules.QueryCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.QueryCustomer.EndPoints;

public class QueryCashCustomerEndpoint : IEndpoint<IResult, QueryCashCustomerRequest, IQueryCashCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/cash-customer", ([AsParameters] QueryCashCustomerRequest request, IQueryCashCustomerService service)
            => HandleAsync(request, service))
            .Produces<CustomerSettingModel>().WithTags("Cash Service");
    }

    public async Task<IResult> HandleAsync(QueryCashCustomerRequest request, IQueryCashCustomerService service)
    {
        if (string.IsNullOrWhiteSpace(request.Language))
        {
            request.Language = "en-US";
        }

        return Results.Ok(await service.Query(request));
    }
}