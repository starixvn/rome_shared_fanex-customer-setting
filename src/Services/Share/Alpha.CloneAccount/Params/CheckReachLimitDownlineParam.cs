namespace Alpha.CloneAccount.Params
{
    public class CheckReachLimitDownlineParam
    {
        public CheckReachLimitDownlineParam(
            int custId,
            byte custLevel,
            string username,
            string language)
        {
            CustId = custId;
            CustLevel = custLevel;
            Username = username;
            Language = language;
        }

        public int CustId { get; }

        public byte CustLevel { get; }

        public string Username { get; }

        public string Language { get; }

        public string LanguageKey { get; }
    }
}