using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetBetSettingEndpoint : IEndpoint<IResult, GetBetSettingRequest, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/bet-setting", ([AsParameters]GetBetSettingRequest request, ICustomerService customerService)
            => HandleAsync(request, customerService))
            .Produces<IEnumerable<SabaSport>>().WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(GetBetSettingRequest request, ICustomerService customerService)
    {
        return Results.Ok(await customerService.GetBetLimitInformation(request));
    }
} 