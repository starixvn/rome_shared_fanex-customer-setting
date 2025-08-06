using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria.Cash;

public class GetCashAccumCredit : INonQueryCommand
{
    public GetCashAccumCredit(int custId) => CustId = custId;

    public int CustId { get; }

    public decimal AccumCreditReturn { get; set; }

    public string GetSettingKey() => "Age_CashAccumCredit_AgSel";

    public bool IsValid() => CustId > 0;
}
