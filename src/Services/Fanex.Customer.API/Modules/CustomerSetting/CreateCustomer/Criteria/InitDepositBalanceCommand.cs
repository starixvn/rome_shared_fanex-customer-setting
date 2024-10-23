using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class InitDepositBalanceCommand : NonQueryCommand
{
    public InitDepositBalanceCommand(int custId, string siteDBName)
    {
        CustId = custId;
        SiteDBName = siteDBName;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    public string SiteDBName { get; }

    public int ErrCode { get; set; }

    public override string GetSettingKey() => $"{SiteDBName}_Dot_BalanceDebitInitialize";

    public override bool IsValid() => CustId > 0;
}