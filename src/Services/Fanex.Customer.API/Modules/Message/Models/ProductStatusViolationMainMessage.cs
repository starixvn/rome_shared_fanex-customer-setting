using System.Xml.Serialization;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

[XmlRoot("Root")]
public class ProductStatusViolationMainMessage : MainMessage<ProductStatusViolationSubMessage>
{
    public IEnumerable<SubMessageContent> TranslateSubMessages(ICacheRefService cacheRefService, string language, string templateMessage)
    {
        if (SubMessages == null || !SubMessages.Any())
        {
            return Enumerable.Empty<SubMessageContent>();
        }

        var messages = new List<SubMessageContent>();
        var parentSportTypeGroups = SubMessages.GroupBy(row => new
        {
            row.ParentSportType,
            row.ParentBetType,
        });

        foreach (var parentSportType in parentSportTypeGroups)
        {
            var parentSportTypeName = cacheRefService.GetSportTypeName(parentSportType.Key.ParentSportType, language);
            var childSportTypeNames = parentSportType
                .Select(subMessage => cacheRefService.GetSportTypeName(subMessage.SportType, language)).ToList();

            messages.Add(new SubMessageContent(string.Format(templateMessage, string.Join(", ", childSportTypeNames), parentSportTypeName)));
        }

        return messages;
    }
}
