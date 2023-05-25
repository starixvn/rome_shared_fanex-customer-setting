using Fanex.Customer.API.Modules.CreateCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, int, string, IQueryCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/pt-settings/{language}", (int custId, string language, IQueryCustomerService customerService)
            => HandleAsync(custId, language, customerService))
            .WithTags("Create Customer");
    }

    public async Task<IResult> HandleAsync(int custId, string language, IQueryCustomerService customerService)
    {
        return Results.Ok(await customerService.GetPTSettings(custId, language));
    }
}