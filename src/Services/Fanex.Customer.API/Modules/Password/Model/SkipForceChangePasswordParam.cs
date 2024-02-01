namespace Fanex.Customer.API.Modules.Password.Model
{
    public class SkipForceChangePasswordParam
    {
        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public string LanguageKey { get; set; }

        public string IP { get; set; }
    }
}