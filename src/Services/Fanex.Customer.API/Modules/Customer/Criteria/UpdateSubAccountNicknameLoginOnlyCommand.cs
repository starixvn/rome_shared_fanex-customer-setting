namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Customer.API.Modules.Customer.Requests;
    using Fanex.Customer.API.Shared.Utils;
    using Fanex.Data;

    public class UpdateSubAccountNicknameLoginOnlyCommand : UpdateNicknameLoginOnlyCommand
    {
        public UpdateSubAccountNicknameLoginOnlyCommand(UpdateNicknameLoginOnlyParam param)
            : base(param)
        {
            SubAccId = param.SubAccountId;
        }

        public int SubAccId { get; }

        [SpParam(Ignored = true)]
        public int CustDBGroupID { get; set; }

        public override string GetSettingKey()
            => SPNameUtils.GetSPNameWithPrefix("Age_CustSubUpdNicknameLoginOnly", CustDBGroupID);
    }
}
