using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Utils;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class WrongPTSubMessage : SubMessage
{
    [XmlAttribute]
    public bool IsLive { get; set; }

    public override string ToString(ICacheRefService cacheRefService, byte settingLevel, string language)
        => $"{base.ToString(cacheRefService, settingLevel, language)} ({StaticResources.Get("Live", language)})";
}

public class MaxPTViolationSubMessage : WrongPTSubMessage
{
    [XmlAttribute]
    public decimal MaxPT { get; set; }

    public override string ToString(ICacheRefService cacheRefService, byte settingLevel, string language)
        => $"{base.ToString(cacheRefService, settingLevel, language)} - {StaticResources.Get(IsLive ? "MaxLive" : "Max", language)}: {PTFormatter.FormatPT(MaxPT)}";
}