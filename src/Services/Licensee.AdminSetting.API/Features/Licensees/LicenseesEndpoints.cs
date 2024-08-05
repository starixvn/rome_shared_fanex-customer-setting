using Licensee.AdminSetting.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;

namespace Licensee.AdminSetting.API.Features.Licensees
{
    public static class LicenseesEndpoints
    {
        public static IEndpointRouteBuilder RegisterLicenseesEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var licensees = endpoints.MapGroup("/licensees");

            licensees.MapPost("/", CreateLicensee);

            return endpoints;
        }

        static async Task<Results<Ok<Result>, ValidationProblem>> CreateLicensee(CreateLicenseeRequest request)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            return TypedResults.Ok(new Result(0, ""));
        }

    }
}