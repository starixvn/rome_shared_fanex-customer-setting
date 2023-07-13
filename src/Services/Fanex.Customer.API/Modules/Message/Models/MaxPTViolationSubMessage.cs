using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Utils;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class MaxPTViolationSubMessage : WrongPTSubMessage
{
    [XmlAttribute]
    public decimal MaxPT { get; set; }

    public override string ToString(ICacheRefService cacheRefService, string language)
        => string.Format("{0} - {1}: {2}",
            base.ToString(cacheRefService, language),
            StaticResources.Get(IsLive ? "MaxLive" : "Max", language),
            PTFormatter.FormatPT(MaxPT));
}
