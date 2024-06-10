namespace Fanex.Customer.API.Modules.SubAccount.Model
{
    public class UpdateSubAccountPageAccessModel
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string PageAccess { get; set; }

        public string IP { get; set; }

        public string Language { get; set; }
    }
}