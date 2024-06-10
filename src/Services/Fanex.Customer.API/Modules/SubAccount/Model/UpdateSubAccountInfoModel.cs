namespace Fanex.Customer.API.Modules.SubAccount.Model
{
    public class UpdateSubAccountInfoModel
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public bool Closed { get; set; }

        public string IP { get; set; }

        public string Language { get; set; }
    }
}