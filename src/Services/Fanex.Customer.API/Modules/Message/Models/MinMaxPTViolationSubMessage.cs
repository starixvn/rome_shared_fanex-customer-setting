using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Utils;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class MinMaxPTViolationSubMessage : WrongPTSubMessage
{
    [XmlAttribute]
    public decimal MinPT { get; set; }

    [XmlAttribute]
    public decimal MaxPT { get; set; }

    public override SubMessageContent ToContent(ICacheRefService cacheRefService, string language)
        => new SubMessageContent(ToString(cacheRefService, language), IsLive);

    public override string ToString(ICacheRefService cacheRefService, string language)
        => string.Format(
            "{0} - {1}: {2} - {3}: {4}",
            base.ToString(cacheRefService, language),
            StaticResources.Get("Min", language),
            PTFormatter.FormatPT(MinPT),
            StaticResources.Get("Max", language),
            PTFormatter.FormatPT(MaxPT));
}
