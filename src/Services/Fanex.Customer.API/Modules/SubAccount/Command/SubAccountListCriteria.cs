using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command
{
    public class SubAccountListCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey()
        {
            return "Age_CustSubSel";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}