using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message;

public interface IMessageService
{
    MessageContent TranslateMessage(int messageType, string rawMessage, string language);
}

public class MessageService : IMessageService
{
    private readonly ICacheRefService cacheRefService;

    private readonly int TimeDiffGMT_4ToGMT8 = 12;

    public MessageService(ICacheRefService cacheRefService)
    {
        this.cacheRefService = cacheRefService;
    }

    public MessageContent TranslateMessage(int messageType, string rawMessage, string language)
        => messageType switch
        { 
            MessageTypes.PlainMessage
                => new MessageContent(rawMessage),
            MessageTypes.WrongAutoPT
                => TranslateSimpleMessage(rawMessage, language),
            MessageTypes.ProductStatusDependencyViolation
                => TranslateProductStatusDependencyViolationSubMessages(rawMessage, language),
            MessageTypes.DownlinePTViolation
                => TranslateDownlinePTViolationSubMessages(rawMessage, language),
            MessageTypes.WrongPT or MessageTypes.WrongPT_Edit
                => TranslateMessage<WrongPTSubMessage>(rawMessage, language),
            MessageTypes.MinPTViolation
                => TranslateMessage<MinPTViolationSubMessage>(rawMessage, language),
            MessageTypes.MaxPTViolation or MessageTypes.MaxPTViolation_Edit
                => TranslateMessage<MaxPTViolationSubMessage>(rawMessage, language),
            MessageTypes.MinMaxPTViolation or MessageTypes.MinMaxPTViolation_Edit
                => TranslateMessage<MinMaxPTViolationSubMessage>(rawMessage, language),
            MessageTypes.SBCommissionViolation
                => TranslateMessage<SBCommissionViolationSubMessage>(rawMessage, language),
            MessageTypes.SBMemberCommissionViolation
                => TranslateMessage<SBMemberCommissionViolationSubMessage>(rawMessage, language),
            MessageTypes.ParlayMaxPayoutViolation
                => TranslateMessage<ParlayMaxPayoutViolationSubMessage>(rawMessage, language),
            MessageTypes.SportsbookBetSettingViolation
                => TranslateMessage<SportsbookBetSettingViolationSubMessage>(rawMessage, language),
            MessageTypes.BetSettingViolation
                => TranslateMessage<BetSettingViolationSubMessage>(rawMessage, language),
            MessageTypes.CommissionViolation
            or MessageTypes.WinLossLimitViolation
            or MessageTypes.ProductStatusViolation
            or MessageTypes.StakeLevelViolation
            or MessageTypes.WinLimitViolatedWithUpline
            or MessageTypes.LossLimitViolatedWithUpline
            or MessageTypes.DailyAutoResetViolatedWithUpline
                => TranslateMessage<SubMessage>(rawMessage, language),
            MessageTypes.StakeLevelMaxChoiceViolation
                => TranslateStakeLevelMaxChoiceViolationSubMessages(rawMessage, language),
            MessageTypes.UpdateTooOften
                => TranslateUpdateTooOftenMessage(rawMessage, language),
            _ => new MessageContent(rawMessage)
        };

    private static MessageContent TranslateSimpleMessage(string rawMessage, string language)
    {
        var mainMessage = rawMessage;
        var simpleMessage = Deserialize<RootMessage>(rawMessage);

        if (simpleMessage != null)
        {
            mainMessage = GetMessage(simpleMessage.MessageId, simpleMessage.Message, language);
        }

        return new MessageContent(mainMessage);
    }

    private MessageContent TranslateMessage<T>(string rawMessage, string language) where T : SubMessage
    {
        var message = Deserialize<MainMessage<T>>(rawMessage);
        var mainMessage = GetMessage(message.MessageId, message.Message, language);
        mainMessage = string.IsNullOrEmpty(mainMessage) ? message.Message : mainMessage;
        if (message.SubMessages != null && message.SubMessages.Any())
        {
            return new MessageContent(mainMessage, TranslateSubMessages(message.SubMessages, language));
        }

        return new MessageContent(mainMessage);
    }

    private IEnumerable<SubMessageContent> TranslateSubMessages<T>(T[] subMessages, string language) where T : SubMessage
    {
        if (subMessages == null || !subMessages.Any())
        {
            return null;
        }
        
        var messages = new List<SubMessageContent>();

        foreach (var subMessage in subMessages)
        {
            messages.Add(subMessage.ToContent(cacheRefService, language));
        }

        return messages;
    }

    private MessageContent TranslateStakeLevelMaxChoiceViolationSubMessages(string rawMessage, string language)
    {
        var messages = Deserialize<MainMessage<StakeLevelMaxChoiceViolationSubMessage>>(rawMessage);
        var translatedMainMessage = string.Format(
                GetMessage(messages.MessageId, messages.Message, language),
                messages.SubMessages[0].MaxChoice,
                cacheRefService.GetSportTypeName(messages.SubMessages[0].SportType, language));
        return new MessageContent(translatedMainMessage);
    }

    private MessageContent TranslateDownlinePTViolationSubMessages(string rawMessage, string language)
    {
        var messages = Deserialize<DownlinePTViolationMainMessage>(rawMessage);
        messages.GroupUsernameBySportTypeBetType();
        var mainMessage = GetMessage(messages.MessageId, messages.Message, language);
        var subMessages = TranslateSubMessages(messages.SubMessages, language);
        return new MessageContent(mainMessage, subMessages);
    }

    private MessageContent TranslateUpdateTooOftenMessage(string rawMessage, string language)
    {
        var mainMessage = rawMessage;
        var updateTooOftenMessage = Deserialize<MainMessage<SubMessage>>(rawMessage);

        if (updateTooOftenMessage != null)
        {
            DateTime lastUpdatedTime;
            if (DateTime.TryParse(updateTooOftenMessage.Message, out lastUpdatedTime))
            {
                mainMessage = string.Format(GetMessage(updateTooOftenMessage.MessageId, updateTooOftenMessage.Message, language), lastUpdatedTime.AddHours(TimeDiffGMT_4ToGMT8).ToString("dd/MM/yyyy"));
            }
        }

        var subMessages = TranslateSubMessages(updateTooOftenMessage.SubMessages, language);
        return new MessageContent(mainMessage, subMessages);
    }

    private MessageContent TranslateProductStatusDependencyViolationSubMessages(string rawMessage, string language)
    {
        var messages = Deserialize<ProductStatusViolationMainMessage>(rawMessage);
        var subMessages = messages.TranslateSubMessages(cacheRefService, language, GetMessage(messages.MessageId, messages.Message, language));
        return new MessageContent(string.Empty, subMessages);
    }

    private static string GetMessage(int messageId, string defaultMessage, string language)
    {
        return StaticResources.Get($"Msg_{messageId}", language) ?? defaultMessage;
    }

    private static T Deserialize<T>(string text) where T : RootMessage
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringReader = new StringReader(text);
        return (T)serializer.Deserialize(stringReader);
    }
}