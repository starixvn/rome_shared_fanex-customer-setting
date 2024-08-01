using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command;

public class AddMsgAfterCreateNewSMACriteria(int custId, int subAccId, int roleid) : NonQueryCommand
{
    public int CustId => custId;

    public int SubAccId => subAccId;

    public int RoleId => roleid;

    public override string GetSettingKey() => "Age_MsgCustReadStatus_Ins";

    public override bool IsValid() => CustId > 0 && SubAccId > 0 && RoleId > 0;
}