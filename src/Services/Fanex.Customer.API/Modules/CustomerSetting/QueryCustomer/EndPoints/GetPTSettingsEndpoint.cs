using Fanex.Customer.API.Modules.QueryCustomer.Requests;
using Fanex.Customer.API.Modules.QueryCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.QueryCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, QueryPTTemplateRequest, IPTSettingService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/pt-settings", ([AsParameters]QueryPTTemplateRequest request, IPTSettingService service)
            => HandleAsync(request, service))
            .WithTags(EnpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(QueryPTTemplateRequest request, IPTSettingService service)
    {
        return Results.Ok(await service.GetPTSettings(request));
    }
}