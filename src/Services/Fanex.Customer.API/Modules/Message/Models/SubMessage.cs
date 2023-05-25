using System.Xml.Serialization;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class SubMessage
{
    [XmlAttribute]
    public short SportType { get; set; }

    [XmlAttribute]
    public short BetType { get; set; }

    public virtual string ToString(ICacheRefService cacheRefService, byte settingLevel, string language)
        => settingLevel switch
        {
            SettingLevel.Product => string.Empty,
            SettingLevel.SportType => cacheRefService.GetSportTypeName(SportType, language),
            SettingLevel.BetType => cacheRefService.GetBetTypeName(BetType, language),
            SettingLevel.SportTypeBetType => $"{cacheRefService.GetSportTypeName(SportType, language)} - {cacheRefService.GetBetTypeName(BetType, language)}",
            _ => throw new InvalidOperationException("Not implement setting level")
        };
}