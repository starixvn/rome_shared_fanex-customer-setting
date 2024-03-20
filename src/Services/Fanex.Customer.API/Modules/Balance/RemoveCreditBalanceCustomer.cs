using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Balance;

public class RemoveCreditBalanceCustomer : NonQueryCommand
{
    public RemoveCreditBalanceCustomer(int custId, string siteDBName)
    {
        CustId = custId;
        SiteDBName = siteDBName;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    public string SiteDBName { get; }

    public override string GetSettingKey() => $"{SiteDBName}_Dot_BalanceCreditRemove";

    public override bool IsValid() => CustId > 0;
}