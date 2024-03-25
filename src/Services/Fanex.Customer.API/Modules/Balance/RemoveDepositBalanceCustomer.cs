using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Balance;

public class RemoveDepositBalanceCustomer : NonQueryCommand
{
    public RemoveDepositBalanceCustomer(int custId, string siteDBName)
    {
        CustId = custId;
        SiteDBName = siteDBName;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    public string SiteDBName { get; }

    public override string GetSettingKey() => $"{SiteDBName}_Dot_BalanceDebitRemove";

    public override bool IsValid() => CustId > 0;
}