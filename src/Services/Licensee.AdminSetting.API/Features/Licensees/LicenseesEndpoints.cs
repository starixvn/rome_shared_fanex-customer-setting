using Licensee.AdminSetting.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;

namespace Licensee.AdminSetting.API.Features.Licensees
{
    public static class LicenseesEndpoints
    {
        public static IEndpointRouteBuilder RegisterLicenseesEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/licensees", CreateLicensee);
            endpoints.MapPost("/api/currencies", CreateCurrency);

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

        static async Task<Results<Ok<Result>, ValidationProblem>> CreateCurrency(
            CreateCurrencyRequest request,
            ICurrencyHandler currencyHandler)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            var result = await currencyHandler.CreateCurrency(request);

            return TypedResults.Ok(result);
        }
    }
}