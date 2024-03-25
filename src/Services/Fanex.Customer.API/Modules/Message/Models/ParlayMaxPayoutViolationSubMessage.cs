using System.Xml.Serialization;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class ParlayMaxPayoutViolationSubMessage : SubMessage
{
    [XmlAttribute]
    public int GroupID { get; set; }

    [XmlAttribute]
    public int GroupType { get; set; }

    public override SubMessageContent ToContent(ICacheRefService cacheRefService, string language)
        => new SubMessageContent(string.Format($"{cacheRefService.GetNameByGroupSubGroup(GroupID, GroupType, SportType, BetType, language)}"));
}
