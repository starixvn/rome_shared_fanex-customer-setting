using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetPTSettingInfoCriteria : CriteriaBase
{
    public GetPTSettingInfoCriteria(int? custId, bool isTest, string site)
    {
        CustId = custId;
        IsTest = isTest;
        Site = site;
    }

    public int? CustId { get; }

    public bool IsTest { get; }

    public string Site { get; }

    public override bool IsValid() => true;

    public override string GetSettingKey()
        => "Acc_CustomerPTInfoSel";
}
