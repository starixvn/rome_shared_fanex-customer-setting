using System.Net;
using System.Text;
using System.Text.Json;
using Deposit.ApiGateways.Models;

namespace Deposit.ApiGateways
{
    public static class ApiKeyErrorType
    {
        public const string API_KEY_REQUIRED = "API key is required";
        public const string API_KEY_INVALID = "API key is invalid";
    }

    public class ApiKeyHandler(IConfiguration configuration) : DelegatingHandler
    {
        private readonly IConfiguration configuration = configuration;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = ValidateApiKey(request);
            if (result.IsNotOk)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { code = HttpStatusCode.Unauthorized, type = result.Type, message = result.Message }),
                        Encoding.UTF8,
                        "application/json")
                };
                return Task.FromResult(response);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private Result ValidateApiKey(HttpRequestMessage request)
        {
            if (request.Headers.Contains("ApiKey") && request.Headers.TryGetValues("ApiKey", out IEnumerable<string> myHeaderList))
            {
                var apiKey = myHeaderList.First();
                var secretKey = configuration["SecretKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    return Result.CreateErrorResult(nameof(ApiKeyErrorType.API_KEY_REQUIRED), ApiKeyErrorType.API_KEY_REQUIRED);
                }

                //do something with the header value
                if (apiKey != secretKey)
                {
                    return Result.CreateErrorResult(nameof(ApiKeyErrorType.API_KEY_INVALID), ApiKeyErrorType.API_KEY_REQUIRED);
                }

                return Result.CreateSuccessfulResult();
            }

            return Result.CreateErrorResult(nameof(ApiKeyErrorType.API_KEY_REQUIRED), ApiKeyErrorType.API_KEY_REQUIRED);
        }
    }


}