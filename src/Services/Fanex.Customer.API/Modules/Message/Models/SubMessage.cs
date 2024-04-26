using System.Xml.Serialization;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class SubMessage
{
    [XmlAttribute("ProductID")]
    public int ProductId { get; set; }

    [XmlAttribute]
    public int SportType { get; set; }

    [XmlAttribute]
    public int BetType { get; set; }

    [XmlAttribute]
    public int SettingLevel { get; set; }

    public virtual SubMessageContent ToContent(ICacheRefService cacheRefService, string language)
    => new SubMessageContent(ToString(cacheRefService, language), false, SportType, BetType);

    public virtual string ToString(ICacheRefService cacheRefService, string language)
        => SettingLevel switch
        {
            Models.SettingLevel.Product => cacheRefService.GetProductName(ProductId, language),
            Models.SettingLevel.SportType => cacheRefService.GetSportTypeName(SportType, language),
            Models.SettingLevel.BetType => cacheRefService.GetBetTypeName(BetType, language),
            _ => $"{cacheRefService.GetSportTypeName(SportType, language)} - {cacheRefService.GetBetTypeName(BetType, language)}"
        };
}