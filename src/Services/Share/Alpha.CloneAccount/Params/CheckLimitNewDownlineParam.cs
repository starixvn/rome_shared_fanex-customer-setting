using Alpha.CloneAccount.Models;

namespace Alpha.CloneAccount.Params
{
    public class CheckLimitNewDownlineParam
    {
        public CheckLimitNewDownlineParam(int custId, UserLevel custLevel, string language)
        {
            CustId = custId;
            CustLevel = custLevel;
            Language = language;
        }

        public int CustId { get; }

        public UserLevel CustLevel { get; }

        public string Language { get; }
    }
}
