using Fanex.Customer.API.Modules.Message.Models;

namespace Fanex.Customer.API.Modules.CopyCustomer.Models;

public class CopyCustomerResult
{
    public CopyCustomerResult(int custId, int code = 0, int msgType = 0, string mainMessage = null)
        : this(custId, code, msgType)
    {
        MessageContent = new MessageContent(mainMessage);
    }

    public CopyCustomerResult(int custId, int code = 0, int msgType = 0, MessageContent messageContent = null)
        : this(custId, code, msgType)
    {
        MessageContent = messageContent;
    }

    public CopyCustomerResult(int custId, int code = 0, int msgType = 0)
    {
        CustId = custId;
        Code = code;
        MsgType = msgType;
    }

    public int CustId { get; }

    public int Code { get; }

    public int MsgType { get; }

    public MessageContent MessageContent { get; }

    public bool IsSuccessful => Code == 0;
}