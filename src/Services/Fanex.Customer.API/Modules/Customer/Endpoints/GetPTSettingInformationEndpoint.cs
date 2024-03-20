using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetPTSettingInformationEndpoint : IEndpoint<IResult, GetPTSettingInformationRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/pt-setting-information", ([AsParameters]GetPTSettingInformationRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<List<PTSettingInfo>>();
    }

    public async Task<IResult> HandleAsync(GetPTSettingInformationRequest request, ICustomerService customerService)
    {
        if (string.IsNullOrWhiteSpace(request.Language))
        {
            request.Language = "en-US";
        }
        return Results.Ok(await customerService.GetPTSettingInfo(request));
    }
}