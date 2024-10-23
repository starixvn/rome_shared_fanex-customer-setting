using Licensee.AdminSetting.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;

namespace Licensee.AdminSetting.API.Features.Accounts
{
    public static class AccountsEndpoints
    {
        public static IEndpointRouteBuilder RegisterAccountsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/dummy-players", CreateDummyPlayer);

            return endpoints;
        }

        static async Task<Results<Ok<Result>, Ok<CreateDummyMemberResult>, ValidationProblem>> CreateDummyPlayer(CreateDummyMemberRequest request, IAccountsHandler accountsHandler)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            var result = await accountsHandler.CreateDummyMember(request);

            return TypedResults.Ok(result);
        }

    }
}