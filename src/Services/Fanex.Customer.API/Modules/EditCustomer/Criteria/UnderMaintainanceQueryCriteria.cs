using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.EditCustomer.Criteria;

internal class UnderMaintainanceQueryCriteria : CriteriaBase
{
    public byte ProductId { get; set; }

    public int CustId { get; set; }

    [SpParam(SqlParamName = "SettingType")]
    public int? SettingTypeId { get; set; }

    public byte FunctionType => 1;

    public override string GetSettingKey()
    {
        return "Acc_CheckUM";
    }

    public override bool IsValid()
    {
        return ProductId > 0 && CustId > 0;
    }
}
