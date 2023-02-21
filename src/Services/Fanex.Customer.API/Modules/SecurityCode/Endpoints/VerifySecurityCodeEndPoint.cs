using Alpha.CloneAccount.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.SecurityCode.Endpoints;

public class VerifySecurityCodeEndPoint : IEndpoint<IResult, VerifySecurityCodeParam, ISecurityCodeService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/security-code/verify", (VerifySecurityCodeParam request, ISecurityCodeService securityCodeService)
            => HandleAsync(request, securityCodeService))
            .Produces<Result>();
    }

    public async Task<IResult> HandleAsync(VerifySecurityCodeParam request, ISecurityCodeService securityCodeService)
    {
        if (request.IsValid())
        {
            return Results.BadRequest();
        }

        var response = await securityCodeService.VerifySecurityCode(request);
        return Results.Ok(response);
    }
}