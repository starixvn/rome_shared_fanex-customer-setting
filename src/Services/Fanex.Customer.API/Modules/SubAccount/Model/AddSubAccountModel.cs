namespace Fanex.Customer.API.Modules.SubAccount.Model
{
    public class AddSubAccountModel
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string PageAccess { get; set; }

        public string IP { get; set; }

        public string Language { get; set; }

        public int RoleId { get; set; }

        public int CustDBGroupID { get; set; }
    }
}