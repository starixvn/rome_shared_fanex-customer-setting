namespace BetLimitControl.API.Authentication
{
    public interface IApiKeyValidation
    {
        bool IsValid(string apiKey);
    }

    public class ApiKeyValidation(IConfiguration configuration) : IApiKeyValidation
    {
        public const string ApiKeyName = "ApiKey";
        private readonly IConfiguration _configuration = configuration;

        public bool IsValid(string userApiKey)
        {
            if (string.IsNullOrWhiteSpace(userApiKey))
            {
                return false;
            }

            var apiKey = _configuration.GetValue<string>(ApiKeyName);

            if (apiKey == null || apiKey != userApiKey)
            {
                return false;
            }

            return true;
        }
    }
}