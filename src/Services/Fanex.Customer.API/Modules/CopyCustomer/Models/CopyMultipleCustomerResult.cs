using Fanex.Customer.API.Modules.Message.Models;

namespace Fanex.Customer.API.Modules.CopyCustomer.Models;

public class CopyMultipleCustomerResult
{

    public CopyMultipleCustomerResult(IEnumerable<string> usernames, int code = 0, int msgType = 0, string mainMessage = null)
        : this(usernames, code, msgType)
    {
        MessageContent = new MessageContent(mainMessage);
    }

    public CopyMultipleCustomerResult(IEnumerable<string> usernames, int code = 0, int msgType = 0, MessageContent messageContent = null)
        : this(usernames, code, msgType)
    {
        MessageContent = messageContent;
    }

    public CopyMultipleCustomerResult(IEnumerable<string> usernames, int code = 0, int msgType = 0)
    {
        Usernames = usernames;
        Code = code;
        MsgType = msgType;
    }

    public int Code { get; }

    public int MsgType { get; }

    public IEnumerable<string> Usernames { get; }

    public MessageContent MessageContent { get; }
}