using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCustomerInfoCriteria : CriteriaBase
{
    public GetCustomerInfoCriteria(int custId, byte settingStatus)
    {
        CustId = custId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey() => "Age_CustomerSelUserInfo";

    public override bool IsValid() => CustId > 0;
}

