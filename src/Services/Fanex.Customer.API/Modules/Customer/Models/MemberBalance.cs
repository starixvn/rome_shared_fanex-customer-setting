namespace Fanex.Customer.API.Modules.Customer.Models
{
    public class MemberBalance
    {
        public int CustId { get; init; }

        public decimal Balance { get; init; }

        public decimal NetPosition { get; init; }

        public decimal CustomerCash => Balance + CustomerOutstanding + PendingTransfer3rd;

        public decimal YesterdayCustomerCash { get; init; }

        public decimal CustomerOutstanding { get; init; }

        public decimal PendingTransfer3rd { get; init; }
    }
}