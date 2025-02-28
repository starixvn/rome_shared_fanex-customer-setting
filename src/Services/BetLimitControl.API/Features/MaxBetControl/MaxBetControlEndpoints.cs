namespace BetLimitControl.API.Features.MaxBetControl
{
    public static class MaxBetControlEndpoints
    {
        public static void RegisterMaxBetControlEndpoints(this WebApplication app)
        {
            var maxBetControl = app.MapGroup("/api/maxbetcontrol");

            maxBetControl.MapGet("/sites", GetAllSites);
            maxBetControl.MapGet("/sites/{siteId}", GetTodoById);
        }

        static async Task<IResult> GetAllSites(IMaxBetControlService maxBetControlService)
        {
            return Results.Ok(await maxBetControlService.GetSites());
        }

        static async Task<IResult> GetTodoById(int siteId, IMaxBetControlService maxBetControlService)
        {
            return Results.Ok(await maxBetControlService.GetCustomerMultipliersBySite(siteId));
        }
    }
}