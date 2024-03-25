using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCommissionHintEndpoint : IEndpoint<IResult, GetCommissionHintRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/commission-hint", ([AsParameters]GetCommissionHintRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<CommissionInfo>();
    }

    public async Task<IResult> HandleAsync(GetCommissionHintRequest request, ICustomerService customerService)
    {
        if (request.CustId < 0)
        {
            return Results.BadRequest();
        }
        
        if (string.IsNullOrWhiteSpace(request.Language))
        {
            request.Language = "en-US";
        }
        
        return Results.Ok(await customerService.GetCommissionInfo(request));
    }
}