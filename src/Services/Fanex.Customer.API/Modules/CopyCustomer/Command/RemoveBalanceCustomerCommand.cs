using Fanex.Data.Repository;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class RemoveBalanceCustomerCommand : NonQueryCommand
{
    public RemoveBalanceCustomerCommand(int custId, bool isCreditBalance)
    {
        CustId = custId;
        IsCreditBalance = isCreditBalance;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    private bool IsCreditBalance { get; }
    public int ErrCode { get; set; }

    public override string GetSettingKey()
        => IsCreditBalance ? "SiteDB_Dot_BalanceCreditRemove" : "SiteDB_Dot_BalanceDebitRemove";

    public override bool IsValid()
        => CustId > 0;
}