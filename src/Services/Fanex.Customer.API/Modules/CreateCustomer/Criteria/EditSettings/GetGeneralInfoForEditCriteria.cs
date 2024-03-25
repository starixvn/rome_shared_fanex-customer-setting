using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings;

public class GetGeneralInfoForEditCriteria : CriteriaBase
{
    public GetGeneralInfoForEditCriteria(int custId, byte systemId)
    {
        CustId = custId;
        SystemId = systemId;
    }

    public int CustId { get; }

    public byte SystemId { get; }

    public override string GetSettingKey()
        => "Acc_CustSetting_CustomerInfoSBSelect";

    public override bool IsValid() => CustId > 0;

}
