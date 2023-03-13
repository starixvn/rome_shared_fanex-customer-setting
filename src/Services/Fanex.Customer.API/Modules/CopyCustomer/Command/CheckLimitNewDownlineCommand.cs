using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class CheckLimitNewDownlineCommand : NonQueryCommand
{
    public CheckLimitNewDownlineCommand(int custId)
    {
        CustId = custId;
    }

    public int CustId { get; }

    public bool IsAllowed { get; set; }

    public override string GetSettingKey()
        => "Acc_ProblemAccountLimitDownlineCheck";

    public override bool IsValid()
        => CustId > 0;
}