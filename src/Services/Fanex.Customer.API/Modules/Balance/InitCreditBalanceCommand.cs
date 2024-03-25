using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Balance;

public class InitCreditBalanceCommand : NonQueryCommand
{
    public InitCreditBalanceCommand(int custId, string siteDBName)
    {
        CustId = custId;
        SiteDBName = siteDBName;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    public string SiteDBName { get; }

    public int ErrCode { get; set; }

    public override string GetSettingKey() => $"{SiteDBName}_Dot_BalanceCreditInitialize";

    public override bool IsValid() => CustId > 0;
}