using System.Collections.Generic;

namespace Fanex.Customer.Client.Models
{

    public class CustomerInfo
    {
        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public string Site { get; set; }

        public string SiteLabel { get; set; }

        public byte RoleId { get; set; }

        public decimal Credit { get; set; }

        public decimal MaxCredit { get; set; }

        public decimal MemberMaxCredit { get; set; }
    }

    public class AccountInfo : CustomerInfo
    {
        public List<string> domains { get; set; }
    }

}