using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.NG2;

public class WriteLogExternalApiUpdResponseCommand : NonQueryCommand
{
    public WriteLogExternalApiUpdResponseCommand(long logId, int returnCode, string response)
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