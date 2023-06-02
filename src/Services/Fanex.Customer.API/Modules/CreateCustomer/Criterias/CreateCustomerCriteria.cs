using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criterias;

public class CreateCustomerCriteria : CriteriaBase
{
    public int CustId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [SpParam(SqlParamName = "Creator")]
    public int CreatorId { get; set; }

    public string UserName { get; set; }

    public string UserPwd { get; set; }

    public string Phone { get; set; }

    public string MobilePhone { get; set; }

    public string Fax { get; set; }

    public bool DisableAutoMPT { get; set; }

    public override string GetSettingKey() => "Acc_LiteMode_CustomerInsMember_SAg";

    public override bool IsValid() => true;
}
