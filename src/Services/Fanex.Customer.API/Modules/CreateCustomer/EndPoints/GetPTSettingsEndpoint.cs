using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, QueryPTTemplateRequest, IPTSettingService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/pt-settings", ([AsParameters]QueryPTTemplateRequest request, IPTSettingService service)
            => HandleAsync(request, service))
            .WithTags("PositionTakingTemplate");
    }

    public async Task<IResult> HandleAsync(QueryPTTemplateRequest request, IPTSettingService service)
    {
        return Results.Ok(await service.GetPTSettings(request));
    }
}