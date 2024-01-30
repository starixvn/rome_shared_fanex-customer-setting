namespace Fanex.Customer.API.Modules.SubAccount.Model
{
    public class SubAccountModel
    {
        public int SubAccId { get; set; }

        public string Username { get; set; }

        public string Nickname { get; set; }

        public string Password { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Closed { get; set; }

        public string PageAccess { get; set; }
    }
}