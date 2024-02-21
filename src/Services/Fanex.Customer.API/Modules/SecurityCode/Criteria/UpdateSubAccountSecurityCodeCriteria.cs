namespace Fanex.Customer.API.Modules.SecurityCode.Criteria
{
    using Fanex.Data;

    public class UpdateSubAccountSecurityCodeCriteria : UpdateSecurityCodeCriteria
    {
        public int SubaccId { get; set; }

        [SpParam(SqlParamName = "securitycode")]
        public string HashedSecurityCode { get; set; }

        public override string GetSettingKey() => "Age_UpdateSubAccountSecurityCode";
    }
}
