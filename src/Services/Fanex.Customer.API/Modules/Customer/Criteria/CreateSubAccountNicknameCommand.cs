namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Customer.API.Modules.Customer.Requests;
    using Fanex.Customer.API.Shared.Utils;
    using Fanex.Data;

    public class CreateSubAccountNicknameCommand : CreateNicknameCommand
    {
        public CreateSubAccountNicknameCommand(CreateNicknameParam param)
            : base(param)
        {
            SubAccId = param.SubAccountId;
        }

        public int SubAccId { get; }

        public override string GetSettingKey()
            => SPNameUtils.GetSPNameWithPrefix("Age_CustSubInsNickname", CustDBGroupID);

        public override bool IsValid()
            => base.IsValid() && SubAccId > 0;
    }
}
