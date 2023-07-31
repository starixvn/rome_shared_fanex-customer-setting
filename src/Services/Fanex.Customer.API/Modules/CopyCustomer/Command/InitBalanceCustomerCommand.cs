using Fanex.Data.Repository;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class InitBalanceCustomerCommand : NonQueryCommand
{
    public InitBalanceCustomerCommand(int custId, bool isCreditBalance)
    {
        CustId = custId;
        IsCreditBalance = isCreditBalance;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    private bool IsCreditBalance { get; }

    public int ErrCode { get; set; }

    public override string GetSettingKey()
        => IsCreditBalance ? "SiteDB_Dot_BalanceCreditInitialize" : "SiteDB_Dot_BalanceDebitInitialize";

    public override bool IsValid()
        => CustId > 0;
}