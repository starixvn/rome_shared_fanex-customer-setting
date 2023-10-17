using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class GetPTTemplateEndpoint : IEndpoint<IResult, QueryPTTemplateRequest, IPTSettingService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/pt-settings", (QueryPTTemplateRequest request, IPTSettingService service)
            => HandleAsync(request, service))
            .WithTags("Create Customer");
    }

    public async Task<IResult> HandleAsync(QueryPTTemplateRequest request, IPTSettingService service)
    {
        return Results.Ok(await service.GetPTSettings(request));
    }
}