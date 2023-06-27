using Fanex.Customer.API.Shared.Constants;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criterias;

public class GetCustomerCriteria : CriteriaBase
{
    public int CustId { get; set; }

    public string Site { get; set; }

    public int GroupId { get; set; }

    public int GroupType { get; set; }

    public byte CreationStatus { get; set; }

    [SpParam(Ignored = true)]
    public int UplineRoleId { get; set; }

    public override string GetSettingKey() 
        => UplineRoleId switch
        {
            ((int)CustomerLevel.Super) =>"Acc_LiteMode_CustSetting_SuperAllInfoSel",
            ((int)CustomerLevel.Master) =>"Acc_LiteMode_CustSetting_MasterAllInfoSel",
            ((int)CustomerLevel.Agent) =>"Acc_LiteMode_CustSetting_MasterAllInfoSel",
            _ => throw new IndexOutOfRangeException($"RoleId cannot = {UplineRoleId}")
        };

    public override bool IsValid() => CustId > 0 && GroupId >= 0 && GroupType >= 0;
}