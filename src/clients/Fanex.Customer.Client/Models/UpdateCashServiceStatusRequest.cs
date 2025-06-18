namespace Fanex.Customer.Client.Models
{
    public class UpdateCashServiceStatusRequest
    {
        public int CustId { get; set; }
        public byte ServiceType { get; set; }
        public string IP { get; set; }
        public string Language { get; set; }
        public string Site { get; set; }
    }
}