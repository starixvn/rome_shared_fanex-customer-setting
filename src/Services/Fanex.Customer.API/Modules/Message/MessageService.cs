using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message;

public interface IMessageService
{
    MessageContent TranslateMessage(int messageType, string rawMessage, byte settingLevel, string language);
}

public class MessageService : IMessageService
{
    private readonly ICacheRefService cacheRefService;

    public MessageService(ICacheRefService cacheRefService)
    {
        this.cacheRefService = cacheRefService;
    }

    public MessageContent TranslateMessage(int messageType, string rawMessage, byte settingLevel, string language)
    {
        return messageType switch
        {
            MessageTypes.MaxPTViolation => TranslateMessage<MaxPTViolationSubMessage>(rawMessage, settingLevel, language),
            _ => new MessageContent(rawMessage)
        };
    }

    private MessageContent TranslateMessage<T>(string rawMessage, byte settingLevel, string language) where T : SubMessage
    {
        var message = Deserialize<MainMessage<T>>(rawMessage);
        var mainMessage = GetMessage(message.MessageId, language);

        if (message.SubMessages.Any())
        {
            return new MessageContent(mainMessage, TranslateSubMessages(message.SubMessages, settingLevel, language));
        }

        return new MessageContent(mainMessage);
    }

    private IEnumerable<string> TranslateSubMessages<T>(T[] subMessages, byte settingLevel, string language) where T : SubMessage
    {
        var messages = new List<string>();

        foreach (var subMessage in subMessages)
        {
            messages.Add(subMessage.ToString(cacheRefService, settingLevel, language));
        }

        return messages;
    }

    private static string GetMessage(int messageId, string language)
    {
        return StaticResources.Get($"Msg_{messageId}", language);
    }

    private static T Deserialize<T>(string text) where T : RootMessage
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringReader = new StringReader(text);
        return (T)serializer.Deserialize(stringReader);
    }
}