namespace Fanex.Customer.API.Modules.Message.Models;

public class MessageContent
{
    public MessageContent(string mainMessage)
    {
        MainMessage = mainMessage;
    }

    public MessageContent(string mainMessage, IEnumerable<SubMessageContent> subMessages)
    {
        MainMessage = mainMessage;
        SubMessages = subMessages;
    }

    public string MainMessage { get; private set; }

    public IEnumerable<SubMessageContent> SubMessages { get; private set; }
}

public class SubMessageContent
{
    public SubMessageContent(string message, bool isLive = false)
    {
        Message = message;
        IsLive = isLive;
    }

    public string Message { get; private set; }

    public bool IsLive { get; set; }
}