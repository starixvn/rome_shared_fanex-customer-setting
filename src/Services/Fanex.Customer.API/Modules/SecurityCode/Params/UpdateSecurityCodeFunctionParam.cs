namespace Fanex.Customer.API.Modules.SecurityCode.Params
{
    public class UpdateSecurityCodeFunctionParam
    {
        public UpdateSecurityCodeFunctionParam(
            bool isOn,
            string function,
            int custId,
            int subAccId,
            string langKey,
            string ip)
        {
            IsOn = isOn;
            Function = function;
            CustId = custId;
            SubAccId = subAccId;
            LangKey = langKey;
            IP = ip;
        }

        public bool IsOn { get; }

        public string Function { get; }

        public int CustId { get; }

        public int SubAccId { get; }

        public string LangKey { get; }

        public string IP { get; }
    }
}
