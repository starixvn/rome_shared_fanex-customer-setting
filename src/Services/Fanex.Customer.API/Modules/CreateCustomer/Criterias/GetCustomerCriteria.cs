using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criterias;

public class GetCustomerCriteria : CriteriaBase
{
    public int CustId { get; set; }

    public string Site { get; set; }

    public int GroupId { get; set; }

    public int GroupType { get; set; }

    public byte CreationStatus { get; set; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_SuperAllInfoSel";

    public override bool IsValid() => CustId > 0 && GroupId >= 0 && GroupType >= 0;
}