using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.EditCustomer.Criteria;

internal class UnderMaintainanceQueryCriteria : CriteriaBase
{
    public int ProductId { get; set; }

    public int CustId { get; set; }

    [SpParam(SqlParamName = "SettingType")]
    public int? SettingTypeId { get; set; }

    public byte FunctionType => 1;

    public byte SettingStatus { get; set; }

    public int GroupId { get; set; }

    public override string GetSettingKey()
    {
        return "Acc_CheckUM";
    }

    public override bool IsValid()
    {
        return CustId > 0;
    }
}
