namespace Fanex.Customer.Client.Models
{
    public class CopyCustomerResult : Result
    {
        public int CustId { get; set; }

        public MessageContent MessageContent { get; set; }

        public int MsgType { get; set; }
    }
}