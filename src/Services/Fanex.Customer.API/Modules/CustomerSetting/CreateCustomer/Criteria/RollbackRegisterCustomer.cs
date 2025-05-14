using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

public class RollbackRegisterCustomer : INonQueryCommand
{
    public RollbackRegisterCustomer(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public string GetSettingKey() => "Acc_CustomerIns_Rollback";
    public bool IsValid() => true;
}
