namespace Fanex.Customer.API.Modules.Customer.Requests
{
    public class UpdateNicknameLoginOnlyParam
    {
        public UpdateNicknameLoginOnlyParam(
            CreateNicknameParam param,
            bool nickNameLoginOnly)
        {
            CustId = param.CustId;
            SubAccountId = param.SubAccountId;
            NickNameLoginOnly = nickNameLoginOnly;
            IP = param.IP;
            LanguageKey = param.LanguageKey;
            IsSubAccount = param.IsSubAccount;
        }

        public int CustId { get; }

        public int SubAccountId { get; }

        public bool NickNameLoginOnly { get; }

        public string IP { get; }

        public string LanguageKey { get; }

        public bool IsSubAccount { get; }
    }
}
