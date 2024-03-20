using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.SecurityCode.Endpoints;

public class GetSecurityCodeEndPoint : IEndpoint<IResult, int, int, ISecurityCodeService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/security-code", (int custId, int subAccId, ISecurityCodeService securityCodeService)
            => HandleAsync(custId, subAccId, securityCodeService))
            .Produces<string>()
            .WithTags("SecurityCode");
    }

    public async Task<IResult> HandleAsync(int custId, int subAccId, ISecurityCodeService securityCodeService)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await securityCodeService.GetSecurityCode(custId, subAccId));
    }
}