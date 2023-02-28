using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class WriteLogExternalAPIUpdResponseCommand : NonQueryCommand
{
    public WriteLogExternalAPIUpdResponseCommand(long logId, int returnCode, string response)
    {
        LogId = logId;
        ReturnCode = returnCode;
        ReturnValue = response;
    }

    public long LogId { get; }

    public int ReturnCode { get; }

    public string ReturnValue { get; }

    public override string GetSettingKey() => "WriteLog_UserLog_ExternalAPI_UpdResponse";

    public override bool IsValid() => true;
}