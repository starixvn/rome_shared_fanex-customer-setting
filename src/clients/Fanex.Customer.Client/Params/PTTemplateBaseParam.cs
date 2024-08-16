namespace Fanex.Customer.Client.Params
{
    public class PTTemplateBaseParam
    {
        public PTTemplateBaseParam(
            int custId,
            int subAccId,
            string language,
            string ip)
        {
            CustId = custId;
            SubAccId = subAccId;
            Language = language;
            IP = ip;
        }

        public int CustId { get; }

        public int SubAccId { get; }

        public string Language { get; }

        public string IP { get; }
    }
}
