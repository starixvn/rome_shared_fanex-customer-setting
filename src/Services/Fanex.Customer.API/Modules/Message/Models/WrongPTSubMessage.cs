using System.Xml.Serialization;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class WrongPTSubMessage : SubMessage
{
    [XmlAttribute]
    public bool IsLive { get; set; }

    public override SubMessageContent ToContent(ICacheRefService cacheRefService, string language)
        => new SubMessageContent(ToString(cacheRefService, language), IsLive);

    public override string ToString(ICacheRefService cacheRefService, string language)
        => ProductId == 1 ? cacheRefService.GetBetTypeName(BetType, language): base.ToString(cacheRefService,language);
}