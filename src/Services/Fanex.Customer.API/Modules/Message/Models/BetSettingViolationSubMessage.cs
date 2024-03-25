using System.Xml.Serialization;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class BetSettingViolationSubMessage : SubMessage
{
    [XmlAttribute]
    public int GroupID { get; set; }

    [XmlAttribute]
    public int GroupType { get; set; }

    public override string ToString(ICacheRefService cacheRefService, string language)
        => string.Format($"{cacheRefService.GetNameByGroupSubGroup(GroupID, GroupType, SportType, BetType, language)}");
}
