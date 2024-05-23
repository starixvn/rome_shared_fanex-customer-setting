using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetMaxPayoutSettingInformationEndpoint : IEndpoint<IResult, GetMaxPayoutSettingInformationRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/max-payout-setting-information", ([AsParameters]GetMaxPayoutSettingInformationRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<MaxPayoutSettingInfo>().WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(GetMaxPayoutSettingInformationRequest request, ICustomerService customerService)
    {
        return Results.Ok(await customerService.GetMaxPayoutSettingInfo(request));
    }
}