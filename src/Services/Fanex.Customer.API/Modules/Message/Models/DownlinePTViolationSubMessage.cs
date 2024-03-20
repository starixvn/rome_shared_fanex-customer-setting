using System.Xml.Serialization;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class DownlinePTViolationSubMessage : WrongPTSubMessage
{
    [XmlAttribute]
    public string UserName { get; set; }

    public override SubMessageContent ToContent(ICacheRefService cacheRefService, string language)
        => new SubMessageContent(string.Format(
            "{0}: {1}",
            base.ToString(cacheRefService, language),
            UserName.TrimEnd(',')), IsLive);
}