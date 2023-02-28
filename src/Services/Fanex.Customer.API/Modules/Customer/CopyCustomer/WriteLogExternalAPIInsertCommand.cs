using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class WriteLogExternalAPIInsertCommand : NonQueryCommand
{
    public WriteLogExternalAPIInsertCommand(
        int systemId,
        int productId,
        int userId,
        string functionName,
        int custId,
        int actionId,
        string request)
    {
        SystemId = systemId;
        ProductId = productId;
        UserId = userId;
        FuncName = functionName;
        CustId = custId;
        ActionId = actionId;
        TrackingValue = request;
    }

    public int SystemId { get; }

    public int ProductId { get; }

    public int UserId { get; }

    public string FuncName { get; }

    public int CustId { get; }

    public int ActionId { get; }

    public string TrackingValue { get; }

    public long LogId { get; set; }

    public override string GetSettingKey() => "WriteLog_UserLog_ExternalAPI_Ins";

    public override bool IsValid() => true;
}