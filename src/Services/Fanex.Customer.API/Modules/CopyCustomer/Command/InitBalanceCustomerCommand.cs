using Fanex.Data.Repository;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class InitBalanceCustomerCommand : NonQueryCommand
{
    public InitBalanceCustomerCommand(int custId, bool isCreditBalance, string prefixSPName)
    {
        CustId = custId;
        IsCreditBalance = isCreditBalance;
        PrefixSPName = prefixSPName;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    private bool IsCreditBalance { get; }

    [SpParam(Ignored = true)]
    private string PrefixSPName { get; }   

    public int ErrCode { get; set; }

    public override string GetSettingKey()
        => $"{PrefixSPName}{(IsCreditBalance ? "Dot_BalanceCreditInitialize" : "Dot_BalanceDebitInitialize")}";

    public override bool IsValid()
        => CustId > 0;
}