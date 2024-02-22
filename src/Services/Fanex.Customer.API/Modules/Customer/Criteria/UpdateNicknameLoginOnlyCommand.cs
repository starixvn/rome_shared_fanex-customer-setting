using Fanex.Customer.API.Modules.Customer.Requests;

namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    public class UpdateNicknameLoginOnlyCommand : NicknameNonQueryCommand
    {
        public UpdateNicknameLoginOnlyCommand(UpdateNicknameLoginOnlyParam param)
            : base(param.CustId, param.IP, param.LanguageKey)
        {
            NickNameLoginOnly = param.NickNameLoginOnly;
        }

        public bool NickNameLoginOnly { get; }

        public override string GetSettingKey()
            => "Age_CustomerUpdNicknameLoginOnly";
    }
}
