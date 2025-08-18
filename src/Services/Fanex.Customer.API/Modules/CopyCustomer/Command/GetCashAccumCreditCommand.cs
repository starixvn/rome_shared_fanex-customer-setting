using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class GetCashAccumCreditCommand : INonQueryCommand
{
    public GetCashAccumCreditCommand(int custId) => CustId = custId;

    public int CustId { get; }

    public decimal AccumCreditReturn { get; set; }

    public string GetSettingKey() => "Age_CashAccumCredit_AgSel";

    public bool IsValid() => CustId > 0;
}
