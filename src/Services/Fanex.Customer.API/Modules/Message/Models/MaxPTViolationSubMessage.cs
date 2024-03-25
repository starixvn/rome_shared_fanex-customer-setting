using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Utils;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class MaxPTViolationSubMessage : WrongPTSubMessage
{
    [XmlAttribute]
    public decimal MaxPT { get; set; }

    public override SubMessageContent ToContent(ICacheRefService cacheRefService, string language)
        => new SubMessageContent(string.Format("{0} - {1}: {2}",
            base.ToString(cacheRefService, language),
            StaticResources.Get("Max", language),
            PTFormatter.FormatPT(MaxPT)), IsLive);
}
