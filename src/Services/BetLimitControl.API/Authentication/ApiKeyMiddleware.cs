using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace BetLimitControl.API.Authentication
{
    public class ApiKeyMiddleware(RequestDelegate next, IApiKeyValidation apiKeyValidation)
    {
        private const string ApiKeyHeaderName = "X-API-Key";
        private readonly RequestDelegate _next = next;
        private readonly IApiKeyValidation _apiKeyValidation = apiKeyValidation;

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
            {
                await _next(context);
                return;
            }

            if (string.IsNullOrWhiteSpace(context.Request.Headers[ApiKeyHeaderName]))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var userApiKey = context.Request.Headers[ApiKeyHeaderName];
            if (!_apiKeyValidation.IsValid(userApiKey!))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next(context);
        }
    }
}