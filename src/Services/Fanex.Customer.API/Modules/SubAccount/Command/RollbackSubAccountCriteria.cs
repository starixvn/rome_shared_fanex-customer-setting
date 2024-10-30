using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command;

public class RollbackSubAccountCriteria(int custId, int subAccId) : NonQueryCommand
{
    public int CustId => custId;

    public int SubAccId => subAccId;

    public override string GetSettingKey() => "Acc_CustomerIns_Rollback";

    public override bool IsValid() => CustId > 0 && SubAccId > 0;
}