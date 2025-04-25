using CashService.API.Features.RegisterCashMember;
using CashService.API.Features.RegisterCashMember.Criteria;
using CashService.API.Features.RegisterCashMember.Requests;
using CashService.API.Models;

public static class RegisterCashMemberEndpoint
{
    public static void MapRegisterCashMemberEndpoint(this WebApplication app)
    {
        app.MapPost("/api/cash-member/register", async (RegisterCashMemberRequest request, IRegisterCashMemberService service) =>
        {
            var result = await RegisterCashMember(request, service);
            return result;
        })
        .WithName("RegisterCashMember")
        .WithTags("CashService");
    }

    public static async Task<IResult> RegisterCashMember(RegisterCashMemberRequest request, IRegisterCashMemberService service)
    => Results.Ok(await service.RegisterCashMember(
        new RegisterCashMemberCommand(
            request.AgentId, 
            request.UserName, 
            request.FirstName, 
            request.LastName, 
            request.PhoneNumber)));
}