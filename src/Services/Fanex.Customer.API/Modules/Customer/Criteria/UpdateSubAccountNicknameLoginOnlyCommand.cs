namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Customer.API.Modules.Customer.Requests;

    public class UpdateSubAccountNicknameLoginOnlyCommand : UpdateNicknameLoginOnlyCommand
    {
        public UpdateSubAccountNicknameLoginOnlyCommand(UpdateNicknameLoginOnlyParam param)
            : base(param)
        {
            SubAccId = param.SubAccountId;
        }

        public int SubAccId { get; }

        public override string GetSettingKey()
            => "Age_CustSubUpdNicknameLoginOnly";

        public override bool IsValid()
            => base.IsValid() && SubAccId > 0;
    }
}
