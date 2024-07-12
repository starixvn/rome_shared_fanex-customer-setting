using System.Net;

namespace Deposit.ApiGateways
{
    public class ApiKeyHandler(IConfiguration configuration) : DelegatingHandler
    {
        private readonly IConfiguration configuration = configuration;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!IsValidKey(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return Task.FromResult(response);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private bool IsValidKey(HttpRequestMessage request)
        {
            if (request.Headers.TryGetValues("ApiKey", out IEnumerable<string> myHeaderList))
            {
                var apiKey = myHeaderList.First();
                var secretKey = configuration["SecretKey"];

                //do something with the header value
                if (!string.IsNullOrEmpty(apiKey) && apiKey == secretKey)
                {
                    return true;
                }
            }

            return false;
        }
    }


}