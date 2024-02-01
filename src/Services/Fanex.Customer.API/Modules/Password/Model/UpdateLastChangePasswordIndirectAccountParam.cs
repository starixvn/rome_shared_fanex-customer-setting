namespace Fanex.Customer.API.Modules.Password.Model
{
    public class UpdateLastChangePasswordIndirectAccountParam
    {
        public int SubAccId { get; set; }

        public int CustId { get; set; }

        public int CustIdIndirect { get; set; }

        public int SubAccIdIndirect { get; set; }

        public string UplineUsername { get; set; }

        public string DownlineUsername { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }
    }
}