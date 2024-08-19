using Licensee.AdminSetting.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;

namespace Licensee.AdminSetting.API.Features.Accounts
{
    public static class AccountsEndpoints
    {
        public static IEndpointRouteBuilder RegisterAccountsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var licensees = endpoints.MapGroup("/api/accounts");

            licensees.MapPost("/{agentId}/dummy-members", CreateDummyMember);

            return endpoints;
        }

        static async Task<Results<Ok<Result>, ValidationProblem>> CreateDummyMember(int agentId, CreateDummyMemberRequest request, IAccountsHandler accountsHandler)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            var result = await accountsHandler.CreateDummyMember(agentId, request);

            return TypedResults.Ok(result);
        }

    }
}