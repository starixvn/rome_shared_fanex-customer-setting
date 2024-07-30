using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Data.Repository;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetAccountInfoEndpoint: IEndpoint<IResult,int, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/account/{custId}", (int custId, ICustomerService customerService)
            => HandleAsync(custId, customerService)).Produces<AccountInfo>();
    }

    public async Task<IResult> HandleAsync(int custId, ICustomerService customerService)
    {
        if (custId > 0)
        {
            var accountInfo = await customerService.GetAccountInfo(custId);
            return Results.Ok(accountInfo);
        }

        return Results.BadRequest();
    }
    
    
}