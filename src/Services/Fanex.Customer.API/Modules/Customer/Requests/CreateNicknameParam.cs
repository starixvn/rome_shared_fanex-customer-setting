namespace Fanex.Customer.API.Modules.Customer.Requests
{
    public class CreateNicknameParam
    {
        public CreateNicknameParam(
            int custId,
            int subAccountId,
            string nickname,
            string ip,
            string languageKey,
            bool isEnableUpdateNickname)
        {
            CustId = custId;
            SubAccountId = subAccountId;
            Nickname = nickname;
            IP = ip;
            LanguageKey = languageKey;
            IsEnableUpdateNickname = isEnableUpdateNickname;
        }

        public int CustId { get; }

        public int SubAccountId { get; }

        public string Nickname { get; }

        public string IP { get; }

        public string LanguageKey { get; }

        public bool IsEnableUpdateNickname { get; }

        public bool IsSubAccount => SubAccountId > 0;
    }
}
