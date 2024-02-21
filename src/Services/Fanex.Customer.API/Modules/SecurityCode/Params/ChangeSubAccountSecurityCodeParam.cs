namespace Fanex.Customer.API.Modules.SecurityCode.Params
{
    public class ChangeSubAccountSecurityCodeParam : ResetSubAccountSecurityCodeParam
    {
        public bool MustVerifyCurrentSecurityCode { get; set; }

        public string CurrentSecurityCode { get; set; }

        public string Salt { get; set; }
    }
}