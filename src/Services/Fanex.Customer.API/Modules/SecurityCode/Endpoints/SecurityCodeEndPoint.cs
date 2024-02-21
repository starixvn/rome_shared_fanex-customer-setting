using Fanex.Customer.API.Modules.SecurityCode.Params;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.SecurityCode.Endpoints;

public class SecurityCodeEndPoint : 
    IEndpoint<IResult, VerifySecurityCodeRequest, ISecurityCodeService>,
    IEndpoint<IResult, int, int, ISecurityCodeService>,
    IEndpoint<IResult, ChangeMainAccountSecurityCodeParam, ISecurityCodeService>,
    IEndpoint<IResult, ChangeSubAccountSecurityCodeParam, ISecurityCodeService>,
    IEndpoint<IResult, ResetSubAccountSecurityCodeParam, ISecurityCodeService>,
    IEndpoint<IResult, UpdateSecurityCodeFunctionParam, ISecurityCodeService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        var securityCode = app.MapGroup("/api/security-code")
            .WithTags("Security Code");

        securityCode.MapGet("/", (int custId, int subAccId, ISecurityCodeService securityCodeService)
            => HandleAsync(custId, subAccId, securityCodeService))
            .Produces<string>();

        securityCode.MapPost("/verify", (VerifySecurityCodeRequest request, ISecurityCodeService securityCodeService)
            => HandleAsync(request, securityCodeService))
            .Produces<Result>();

        securityCode.MapPost("/main-account/change", (ChangeMainAccountSecurityCodeParam param, ISecurityCodeService securityCodeService)
            => HandleAsync(param, securityCodeService))
            .Produces<Result>();

        securityCode.MapPost("/sub-account/change", (ChangeSubAccountSecurityCodeParam param, ISecurityCodeService securityCodeService)
            => HandleAsync(param, securityCodeService))
            .Produces<Result>();

        securityCode.MapPost("/sub-account/reset", (ResetSubAccountSecurityCodeParam param, ISecurityCodeService securityCodeService)
            => HandleAsync(param, securityCodeService))
            .Produces<Result>();

        securityCode.MapPost("/update-function", (UpdateSecurityCodeFunctionParam request, ISecurityCodeService securityCodeService)
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

    public async Task<IResult> HandleAsync(int custId, int subAccId, ISecurityCodeService securityCodeService)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await securityCodeService.GetSecurityCode(custId, subAccId));
    }

    public async Task<IResult> HandleAsync(ResetSubAccountSecurityCodeParam request1, ISecurityCodeService request2)
    {
        if (request1.SubAccId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.ResetSubAccountSecurityCode(request1));
    }

    public async Task<IResult> HandleAsync(ChangeMainAccountSecurityCodeParam request1, ISecurityCodeService request2)
    {
        if (request1.CustId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.ChangeMainAccountSecurityCode(request1));
    }

    public async Task<IResult> HandleAsync(ChangeSubAccountSecurityCodeParam request1, ISecurityCodeService request2)
    {
        if (request1.SubAccId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.ChangeSubAccountSecurityCode(request1));
    }

    public async Task<IResult> HandleAsync(UpdateSecurityCodeFunctionParam request1, ISecurityCodeService request2)
    {
        if (request1.CustId < 0)
        {
            return Results.BadRequest();
        }

        return Results.Ok(await request2.UpdateSecurityCodeFunction(request1));
    }
}