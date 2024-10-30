namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Customer.API.Modules.Customer.Requests;
    using Fanex.Customer.API.Shared.Utils;
    using Fanex.Data;

    public class CreateNicknameCommand : NicknameNonQueryCommand
    {
        public CreateNicknameCommand(CreateNicknameParam param)
            : base(param.CustId, param.IP, param.LanguageKey)
        {
            Nickname = param.Nickname;
            CustDBGroupID = param.CustDBGroupID;
        }

        public string Nickname { get; }

        [SpParam(Ignored = true)]
        public int CustDBGroupID { get; }

        public override string GetSettingKey()
            => SPNameUtils.GetSPNameWithPrefix("Age_CustomerInsNickName", CustDBGroupID);

        public override bool IsValid()
            => base.IsValid()
            && !string.IsNullOrWhiteSpace(Nickname);
    }
}
