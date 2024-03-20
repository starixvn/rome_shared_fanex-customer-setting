using System.Xml.Serialization;

namespace Fanex.Customer.API.Modules.Message.Models;

[XmlRoot("Root")]
public class DownlinePTViolationMainMessage : MainMessage<DownlinePTViolationSubMessage>
{
    public void GroupUsernameBySportTypeBetType()
    {
        SubMessages = SubMessages.GroupBy(row => new
        {
            row.ProductId,
            row.SportType,
            row.BetType,
            row.IsLive,
            row.SettingLevel
        }).Select(group => new DownlinePTViolationSubMessage
        {
            ProductId = group.Key.ProductId,
            SportType = group.Key.SportType,
            BetType = group.Key.BetType,
            IsLive = group.Key.IsLive,
            SettingLevel = group.Key.SettingLevel,
            UserName = string.Join(", ", group.Select(row => row.UserName))
        }).ToArray();
    }
}
