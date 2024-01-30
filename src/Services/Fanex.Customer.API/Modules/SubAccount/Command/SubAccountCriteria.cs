using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command
{
    public class SubAccountCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public override string GetSettingKey()
        {
            return "Age_CustSubSelInfo";
        }

        public override bool IsValid()
        {
            return CustId > 0 && SubAccId > 0;
        }
    }
}