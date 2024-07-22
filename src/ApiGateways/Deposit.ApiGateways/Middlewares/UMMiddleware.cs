using System.Net;
using ONEbook.UM.Services;

namespace Deposit.ApiGateways.Middlewares
{
    public class UMMiddleware(RequestDelegate next, IMaintenanceService maintenanceService)
    {
        private readonly RequestDelegate next = next;
        private readonly IMaintenanceService maintenanceService = maintenanceService;

        public async Task InvokeAsync(HttpContext context)
        {
            var umInfo = await maintenanceService.GetUnderMaintenanceTimeAsync();
            if (umInfo.IsUnderMaintenanceTime)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                var message = string.Format("Sorry, the system is under maintenance from {0} to {1} GMT+8. Please come back later.", umInfo.From, umInfo.To);
                await response.WriteAsync(message);
            }

            await next(context);
        }
    }
}