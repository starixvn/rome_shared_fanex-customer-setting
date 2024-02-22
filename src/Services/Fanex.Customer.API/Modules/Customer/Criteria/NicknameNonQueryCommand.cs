namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Data.Repository;

    public abstract class NicknameNonQueryCommand : NonQueryCommand
    {
        protected NicknameNonQueryCommand(
            int custId,
            string ip,
            string language)
        {
            CustId = custId;
            IP = ip;
            Language = language;
        }

        public int CustId { get; }

        public string IP { get; }

        public string Language { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override bool IsValid()
            => CustId > 0;
    }
}
