namespace Fanex.Customer.API.Modules.Customer.Models
{
    public class LogDataEntity
    {
        public int UserID { get; set; }
        public int CustID { get; set; }
        public int ActionID { get; set; }
        public string TrackingValue { get; set; }
        public string TrackingValue2 { get; set; }
        public string TrackingValueMask { get; set; }
        public string TrackingValueMask2 { get; set; }
        public int Duration { get; set; }
    }
}
