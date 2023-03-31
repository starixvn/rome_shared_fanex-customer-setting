using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class CheckEnableCustomerCommand : NonQueryCommand
{
    public CheckEnableCustomerCommand(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Acc_CustomerSelCheckEnable";

    public override bool IsValid() => CustId > 0;
}