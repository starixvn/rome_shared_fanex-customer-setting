using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.SecurityCode.Endpoints;

public class VerifySecurityCodeEndPoint : IEndpoint<IResult, VerifySecurityCodeRequest, ISecurityCodeService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/security-code/verify", (VerifySecurityCodeRequest request, ISecurityCodeService securityCodeService)
            => HandleAsync(request, securityCodeService))
            .Produces<Result>();
    }

    public async Task<IResult> HandleAsync(VerifySecurityCodeRequest request, ISecurityCodeService securityCodeService)
    {
        if (request.IsValid())
        {
            var response = await securityCodeService.VerifySecurityCode(request);
            return Results.Ok(response);
        }

        return Results.BadRequest();
    }
}