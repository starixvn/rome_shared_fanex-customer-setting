using Fanex.Customer.API.Modules.CreateCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, int, string, IPTSettingService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/pt-settings/{language}", (int custId, string language, IPTSettingService service)
            => HandleAsync(custId, language, service))
            .WithTags("Create Customer");
    }

    public async Task<IResult> HandleAsync(int custId, string language, IPTSettingService service)
    {
        return Results.Ok(await service.GetPTSettings(custId, language));
    }
}