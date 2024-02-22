namespace Fanex.Customer.API.Modules.Customer.Requests
{
    public class UsageLanguageRequest
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string LanguageUsage { get; set; }

        public string IP { get; set; }
    }
}