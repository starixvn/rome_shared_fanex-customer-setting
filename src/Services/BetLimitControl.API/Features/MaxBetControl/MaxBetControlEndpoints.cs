using BetLimitControl.API.Features.MaxBetControl.Models;

namespace BetLimitControl.API.Features.MaxBetControl
{
    public static class MaxBetControlEndpoints
    {
        public static void RegisterMaxBetControlEndpoints(this WebApplication app)
        {
            app.MapGet("/api/sites", GetAllSites)
                .Produces<IEnumerable<int>>()
                .WithTags("Get Sites");

            app.MapGet("/api/sites/{siteId}", GetCustomerMultiplierBySite)
                .Produces<IEnumerable<CustomerMultiplier>>()
                .WithTags("Get Customer Multiplier By Site");
        }

        static async Task<IResult> GetAllSites(IMaxBetControlService maxBetControlService)
        {
            return Results.Ok(await maxBetControlService.GetSites());
        }

        static async Task<IResult> GetCustomerMultiplierBySite(int siteId, IMaxBetControlService maxBetControlService)
        {
            return Results.Ok(await maxBetControlService.GetCustomerMultipliersBySite(siteId));
        }
    }
}