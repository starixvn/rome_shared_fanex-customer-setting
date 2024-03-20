using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Utils;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class MinPTViolationSubMessage : WrongPTSubMessage
{
    [XmlAttribute]
    public decimal MinPT { get; set; }

    public override SubMessageContent ToContent(ICacheRefService cacheRefService, string language)
         => new SubMessageContent(string.Format("{0} - {1}: {2}",
            base.ToString(cacheRefService, language),
            StaticResources.Get("Min", language),
            PTFormatter.FormatPT(MinPT)), IsLive);
}
