using Fanex.Customer.API.Modules.SecurityCode.Params;

namespace Fanex.Customer.API.Modules.SecurityCode.Criteria
{
    public class UpdateSecurityCodeFunctionCriteria : UpdateSecurityCodeCriteria
    {
        public UpdateSecurityCodeFunctionCriteria(UpdateSecurityCodeFunctionParam param)
        {
            IsOn = param.IsOn;
            Function = param.Function;
            CustId = param.CustId;
            SubAccId = param.SubAccId;
            Language = param.LangKey;
            IP = param.IP;
        }

        public bool IsOn { get; }

        public string Function { get; }

        public int SubAccId { get; }

        public override string GetSettingKey()
            => "Age_CustomerUpdSecurityFunctionInfo";

        public override bool IsValid()
            => base.IsValid() && SubAccId >= 0;
    }
}
