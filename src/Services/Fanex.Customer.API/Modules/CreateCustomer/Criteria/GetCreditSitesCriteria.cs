using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class GetCreditSitesCriteria : CriteriaBase
{
    public GetCreditSitesCriteria(int userId)
    {
        UserId = userId;
    }

    public int UserId { get; }

    public override string GetSettingKey() => "Acc_SiteSel";

    public override bool IsValid() => UserId > 0;
}
