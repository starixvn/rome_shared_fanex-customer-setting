namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Customer.API.Modules.Customer.Requests;

    public class CreateSubAccountNicknameCommand : CreateNicknameCommand
    {
        public CreateSubAccountNicknameCommand(CreateNicknameParam param)
            : base(param)
        {
            SubAccId = param.SubAccountId;
        }

        public int SubAccId { get; }

        public override string GetSettingKey()
            => "Age_CustSubInsNickname";

        public override bool IsValid()
            => base.IsValid() && SubAccId > 0;
    }
}
