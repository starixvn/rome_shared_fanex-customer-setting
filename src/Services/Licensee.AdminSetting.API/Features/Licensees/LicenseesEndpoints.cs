using Licensee.AdminSetting.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;

namespace Licensee.AdminSetting.API.Features.Licensees
{
    public static class LicenseesEndpoints
    {
        public static IEndpointRouteBuilder RegisterLicenseesEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var licensees = endpoints.MapGroup("/api/licensees");

            licensees.MapPost("/", CreateLicensee);

            return endpoints;
        }

        static async Task<Results<Ok<Result>, ValidationProblem>> CreateLicensee(CreateLicenseeRequest request, ILicenseesHandler licenseesHandler)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            var result = await licenseesHandler.CreateLicensee(request);

            return TypedResults.Ok(result);
        }

    }
}