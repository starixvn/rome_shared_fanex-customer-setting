using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Password.Service.Model
{
    public class ResetSubAccountPasswordParam
    {
        public int SubAccountId { get; set; }

        public int CustId { get; set; }

        public int ActorId { get; set; }

        public string ActorName { get; set; }

        public string NewPassword { get; set; }

        public string LanguageKey { get; set; }

        public string IP { get; set; }

        public string HashKey { get; set; }

        public UserLevel CustLevel { get; set; }

        public int ServerId { get; set; }

        public bool IsIndirectLoggedIn { get; set; }

        public int IndirectCustId { get; set; }

        public int IndirectSubAccId { get; set; }

        public string IndirectUsername { get; set; }
    }
}