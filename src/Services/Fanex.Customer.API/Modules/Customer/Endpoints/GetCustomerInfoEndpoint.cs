using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Data.Repository;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCustomerInfoEndpoint : IEndpoint<IResult, int, IDynamicRepository>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}", (int custId, IDynamicRepository dynamicRepository)
            => HandleAsync(custId, dynamicRepository))
            .Produces<CustomerInfo>()
            .WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(int custId, IDynamicRepository dynamicRepository)
    {
        if (custId > 0)
        {
            var customerInfo = await dynamicRepository.GetAsync<CustomerInfo>(new GetCustomerInfoCriteria { CustId = custId });
            return Results.Ok(customerInfo);
        }

        return Results.BadRequest();
    }
}
