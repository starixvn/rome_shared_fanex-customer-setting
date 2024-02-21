namespace Fanex.Customer.API.Modules.SecurityCode.Criteria
{
    public class UpdateMainAccountSecurityCodeCounterCriteria : UpdateSecurityCodeCriteria
    {
        public bool CheckedSecurityCode { get; set; }

        public override string GetSettingKey() => "Age_UpdateMainAccountSecurityCodeCounter";
    }
}
