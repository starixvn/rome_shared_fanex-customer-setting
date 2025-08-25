namespace Fanex.Customer.Client.Params
{
    public class UpdateCustomerInfoParam
    {
        public int CustId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public int UserId { get; set; }
        public int SubAccId { get; set; }
        public string IP { get; set; }
        public string Language { get; set; }
        public string Remark { get; set; }
    }
}
