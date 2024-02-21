namespace Fanex.Customer.API.Modules.SecurityCode.Criteria
{
    using Fanex.Data;

    public class UpdateMainAccountSecurityCodeCriteria : UpdateSecurityCodeCriteria
    {
        public int SubaccId { get; set; }

        public int UserId { get; set; }

        [SpParam(SqlParamName = "securitycode")]
        public string HashedSecurityCode { get; set; }

        public override string GetSettingKey() => "Age_UpdateMainAccountSecurityCode";
    }
}
