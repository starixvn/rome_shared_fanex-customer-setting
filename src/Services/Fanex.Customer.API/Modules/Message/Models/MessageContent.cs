using System.Text.Json.Serialization;

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
    public SubMessageContent(string message, bool isLive = false, int sportTypeId = 0, int bettypeId = 0)
    {
        Message = message;
        IsLive = isLive;
        SportTypeId = sportTypeId;
        BettypeId = bettypeId;
    }

    public string Message { get; private set; }

    public bool IsLive { get; set; }

    [JsonIgnore]
    public int SportTypeId { get; set; }

    [JsonIgnore]
    public int BettypeId { get; set; }
}