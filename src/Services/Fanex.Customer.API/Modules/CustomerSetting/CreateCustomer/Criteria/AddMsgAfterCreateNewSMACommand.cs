using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class AddMsgAfterCreateNewSMACommand : INonQueryCommand
{
    public AddMsgAfterCreateNewSMACommand(int custId, int subAccId, int roleid)
    {
        CustId = custId;
        SubAccId = subAccId;
        RoleId = roleid;
    }

    public int CustId { get; }

    public int SubAccId { get; }

    public int RoleId { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Age_MsgCustReadStatus_Ins";

    public bool IsValid() => CustId > 0;
}
