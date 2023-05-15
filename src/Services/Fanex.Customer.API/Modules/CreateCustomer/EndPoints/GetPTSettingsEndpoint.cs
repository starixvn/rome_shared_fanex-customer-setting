using Fanex.Customer.API.Modules.CreateCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, int, string, ICreateCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/pt-settings/{language}", (int custId, string language, ICreateCustomerService customerService)
            => HandleAsync(custId, language, customerService))
            .WithTags("LiteMode");
    }

    public async Task<IResult> HandleAsync(int custId, string language, ICreateCustomerService customerService)
    {
        return Results.Ok(await customerService.GetPTSettings(custId, language));
    }
}