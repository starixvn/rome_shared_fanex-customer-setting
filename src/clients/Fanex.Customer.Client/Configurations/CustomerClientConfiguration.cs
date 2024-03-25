namespace Fanex.Customer.Client.Configurations
{
    public class CustomerClientConfiguration
    {
        public CustomerClientConfiguration()
        {
        }

        public CustomerClientConfiguration(string serviceUri, int systemId, string accessDeniedUrl)
        {
            ServiceUri = serviceUri;
            SystemId = systemId;
            AccessDeniedUrl = accessDeniedUrl;
        }

        public int SystemId { get; }

        public string ServiceUri { get; }

        public string AccessDeniedUrl { get; }
    }
}
