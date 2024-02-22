namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Data.Repository;

    public class GetProtectionCodeCriteria : CriteriaBase
    {
        public GetProtectionCodeCriteria(int custId, int subAccId)
        {
            CustId = custId;
            SubAccId = subAccId;
        }

        public int CustId { get; }

        public int SubAccId { get; }

        public override string GetSettingKey()
            => "Age_CustomerSelProtectionCode";

        public override bool IsValid() => CustId > 0 && SubAccId >= 0;
    }
}
