using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetProductCategoriesEndpoint:IEndpoint<IResult, GetProductCategoriesRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/product-categories", ([AsParameters]GetProductCategoriesRequest request, ICustomerService customerService)
                => HandleAsync(request, customerService))
            .Produces<CreditInfo>().WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(GetProductCategoriesRequest request, ICustomerService customerService)
    {
        if (string.IsNullOrWhiteSpace(request.Language))
        {
            request.Language = "en-US";
        }
        return Results.Ok(await customerService.GetProductCategories(request));
    }
}