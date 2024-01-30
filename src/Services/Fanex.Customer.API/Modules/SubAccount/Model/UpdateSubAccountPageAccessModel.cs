namespace Fanex.Customer.API.Modules.SubAccount.Model
{
    public class UpdateSubAccountPageAccessModel
    {
        public int SubAccId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string PageAccess { get; set; }

        public string IP { get; set; }

        public string Language { get; set; }
    }
}