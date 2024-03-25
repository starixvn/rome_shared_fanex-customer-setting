using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class SiteEndpoint : IEndpoint<IResult, int, ISiteService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sites/{userId}", (int userId, ISiteService service)
            => HandleAsync(userId, service))
            .Produces<Site>().WithTags("Create Customer Full Mode");
    }

    public async Task<IResult> HandleAsync(int userId, ISiteService service)
    {
        return Results.Ok(await service.GetCreditSites(userId));
    }
}