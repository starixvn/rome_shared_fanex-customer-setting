namespace Fanex.Customer.Client.Models
{
    public class Result
    {
        public const int ErrorCode = 1;

        public int Code { get; set; }

        public string Message { get; set; }

        public virtual bool IsSuccessful
        {
            get
            {
                return Code == 0;
            }
        }

        public virtual bool IsFailed
        {
            get
            {
                return !IsSuccessful;
            }
        }
    }
}
