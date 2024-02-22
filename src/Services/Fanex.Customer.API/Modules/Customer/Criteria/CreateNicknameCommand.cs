namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Customer.API.Modules.Customer.Requests;

    public class CreateNicknameCommand : NicknameNonQueryCommand
    {
        public CreateNicknameCommand(CreateNicknameParam param)
            : base(param.CustId, param.IP, param.LanguageKey)
        {
            Nickname = param.Nickname;
        }

        public string Nickname { get; }

        public override string GetSettingKey()
            => "Age_CustomerInsNickName";

        public override bool IsValid()
            => base.IsValid()
            && !string.IsNullOrWhiteSpace(Nickname);
    }
}
