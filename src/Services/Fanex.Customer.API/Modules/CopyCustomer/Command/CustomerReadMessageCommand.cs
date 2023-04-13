using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class CustomerReadMessageCommand : NonQueryCommand
{
    public CustomerReadMessageCommand(int custId, int roleId, int subAccId, int errorCode, string errorMessage)
    {
        CustId = custId;
        RoleId = roleId;
        SubAccId = subAccId;
        ErrCode = errorCode;
        ErrMsg = errorMessage;
    }

    public int CustId { get; }

    public int RoleId { get; }

    public int SubAccId { get; }

    public string ErrMsg { get; set; }

    public int ErrCode { get; set; }

    public override string GetSettingKey() => "Age_MsgCustReadStatus_Ins";

    public override bool IsValid() => CustId > 0 && RoleId > 1;
}